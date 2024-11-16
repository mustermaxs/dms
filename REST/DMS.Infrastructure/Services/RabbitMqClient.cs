using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using DMS.Application.Interfaces;
using DMS.Infrastructure.Configs;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using IConnectionFactory = RabbitMQ.Client.IConnectionFactory;

namespace DMS.Infrastructure.Services
{
    public class RabbitMqClient : IMessageBroker, IDisposable
    {
        private readonly RabbitMqConfig _config;
        private static IConnectionFactory _connectionFactory;
        protected static bool IsInitialized { get; set; }
        private static IConnection _connection;
        private static IChannel _channel;
        private string? _replyQueueName;


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
                await CreateQueue("ocr-process");
                await CreateQueue("ocr-result");
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

        public async Task StartAsync(string queueName)
        {
            await EnsureInitialized();
            await CreateQueue(queueName);
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

        public async Task Subscribe<TResponseObj>(string queueName, Action<TResponseObj> callback)
        {
            await EnsureInitialized();
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var responseObj = JsonSerializer.Deserialize<TResponseObj>(message);
                callback(responseObj!);
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };
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