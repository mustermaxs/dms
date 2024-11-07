using System.Collections.Concurrent;
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
        private readonly IConnectionFactory _connectionFactory;
        protected static bool IsInitialized { get; set; }
        private static IConnection _connection;
        private static IChannel _channel;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper
            = new();

        public RabbitMqClient(RabbitMqConfig config)
        {
            _config = config;
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _config.Host,
                Port = _config.Port,
                UserName = _config.Username,
                Password = _config.Password
            };
        }

        public async Task InitiliazeAsync()
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
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

        public async Task<string> PublishRpc<TMessageObject>(string queueName, TMessageObject message)
        {
            await EnsureInitialized();
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
            
            var openChannel = await _channel.BasicGetAsync(queueName, true);
            if (openChannel == null) throw new RabbitMQ.Client.Exceptions.OperationInterruptedException();

            var reponse = await tcs.Task;
            return reponse;
        }

        public Task Subscribe(string queueName)
        {
            throw new NotImplementedException();
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