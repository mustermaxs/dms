using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using DMS.Application.Interfaces;
using DMS.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DMS.Infrastructure.Services
{
    public class RabbitMqClient : IMessageBroker, IDisposable
    {
        private readonly RabbitMqConfig _config;
        private static IConnectionFactory _connectionFactory;
        protected static bool IsInitialized { get; set; }
        private static IConnection _connection;
        private static IChannel _channel;

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
                UserName = config.UserName
            };
        }

        public async Task InitiliazeAsync()
        {
            try
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task EnsureInitialized()
        {
            if (!_connection.IsOpen || !_channel.IsOpen)
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
                var queueDeclareOk = await _channel.QueueDeclarePassiveAsync(queueName);

                return queueDeclareOk.MessageCount > 0;
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex) when (ex.ShutdownReason is
                {
                    ReplyCode: 404
                })
            {
                return await Task.FromResult(false);
            }
        }

        public async Task StartAsync(string queueName)
        {
            await InitiliazeAsync();
            var replyQueueName = $"{queueName}_reply";
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                string? correlationId = ea.BasicProperties.CorrelationId;

                if (false == string.IsNullOrEmpty(correlationId))
                {
                    if (_callbackMapper.TryRemove(correlationId, out var tcs))
                    {
                        var body = ea.Body.ToArray();
                        var response = Encoding.UTF8.GetString(body);
                        tcs.TrySetResult(response);
                    }
                }

                return Task.CompletedTask;
            };
            await _channel.BasicConsumeAsync(queueName, false, consumer);
        }

        public async Task<string> PublishRpc<TMessageObject>(string queueName, TMessageObject message,
            CancellationToken cancellationToken = default)
        {
            await StartAsync(queueName);
            if (!await QueueExists(queueName)) await CreateQueue(queueName);
            var props = new BasicProperties
            {
                CorrelationId = Guid.NewGuid().ToString(),
                ReplyTo = queueName
            };
            var tcs = new TaskCompletionSource<string>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(props.CorrelationId, tcs);

            var msgContent = JsonSerializer.Serialize<TMessageObject>(message);
            var messageBody = System.Text.Encoding.UTF8.GetBytes(msgContent);
            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                body: messageBody,
                basicProperties: props,
                mandatory: true);

            await using CancellationTokenRegistration ctr =
                cancellationToken.Register(() =>
                {
                    _callbackMapper.TryRemove(props.CorrelationId, out _);
                    tcs.SetCanceled(cancellationToken);
                });
            return await tcs.Task;
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
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += eventHandler;
            await _channel.BasicConsumeAsync(queueName, false, consumer);
        }

        public Task Acknowledge(ulong deliveryTag)
        {
            throw new NotImplementedException();
        }

        public Task Reject(ulong deliveryTag, bool requeue)
        {
            throw new NotImplementedException();
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
}