using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using OcrService;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using OcrService.Configs;
namespace WorkerService1;
// https://www.rabbitmq.com/tutorials/tutorial-six-dotnet

    public class RabbitMqClient : IDisposable
    {
        private readonly RabbitMqConfig _config;
        private static IConnectionFactory _connectionFactory;
        protected static bool IsInitialized { get; set; }
        private static IConnection _connection;
        private static IChannel _channel;
        private string? _replyQueueName;
        private const string QUEUE_NAME = "ocr";


        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper
            = new();

        public RabbitMqClient(RabbitMqConfig config)
        {
            _config = config;
            _connectionFactory = new ConnectionFactory
            {
                HostName = config.HostName,
                Password = config.Password,
                Port = config.Port,
                Endpoint = new AmqpTcpEndpoint(config.Endpoint),
                UserName = config.UserName,
                VirtualHost = "/"
            };
        }

        public async Task InitiliazeAsync()
        {
            try
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await CreateQueue("ocr-process"); // Pass queue name as argument
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task EnsureInitialized()
        {
            if (_connection is null || _channel is null || !_connection.IsOpen || !_channel.IsOpen)
            {
                await InitiliazeAsync();
            }
        }

        protected async Task CreateQueue(string queueName)
        {
            await _channel.QueueDeclareAsync(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        protected async Task<bool> QueueExists(string queueName)
        {
            await EnsureInitialized();
            try
            {
                await _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);

                return true;
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex) when (ex.ShutdownReason is
                {
                    ReplyCode: 404
                })
            {
                await _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);
                return await Task.FromResult(true);
            }
        }



        public async Task Publish<TMessageObject>(string queueName, TMessageObject messageObject)
        {
            await EnsureInitialized();
            var msgContentJson = JsonSerializer.Serialize<TMessageObject>(messageObject);
            var body = Encoding.UTF8.GetBytes(msgContentJson);
            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                body: body,
                mandatory: true);
        }

        public async Task Subscribe(string queueName, AsyncEventHandler<BasicDeliverEventArgs> eventHandler)
        {
            await EnsureInitialized();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_model, ea) =>
            {
                try
                {
                    if (eventHandler != null)
                    {
                        await eventHandler.Invoke(this, ea);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    await _channel.BasicRejectAsync(ea.DeliveryTag, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(queueName, autoAck: false, consumer);
        }

        public Task Acknowledge(ulong deliveryTag)
        {
            throw new NotImplementedException();
        }

        public async Task Reject(ulong deliveryTag, bool requeue)
        {
            await _channel.BasicRejectAsync(deliveryTag, requeue: true);
        }

        public Task Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }