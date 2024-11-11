using System.Text;
using System.Text.Json;
using IronOcr;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkerService1;
// https://www.rabbitmq.com/tutorials/tutorial-six-dotnet
public class RabbitMqServer : IAsyncDisposable
{
    private ConnectionFactory _factory;
    private IConnection _connection;
    private IChannel _channel;
    private readonly string _queueName;

    public RabbitMqServer(string hostName, string queueName)
    {
        _factory = new ConnectionFactory()
        {
            HostName = hostName,
            Port = 5672,
            UserName = "dmsadmin",
            Password = "dmsadmin",
            Endpoint = new AmqpTcpEndpoint("localhost")
        };
        _queueName = queueName;
    }

    private async Task InitializeAsync()
    {
        _connection = await _factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(queue: _queueName, durable:false, exclusive: false, autoDelete: false, arguments: null);
        await _channel.BasicQosAsync(prefetchCount:1, prefetchSize:0, global: false);
    }

    public async Task StartListeningAsync(Func<Stream, Task<string>> requestHandler)
    {
        await InitializeAsync();
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            AsyncEventingBasicConsumer cons = (AsyncEventingBasicConsumer)sender;
            IReadOnlyBasicProperties props = ea.BasicProperties;
            var replyProps = new BasicProperties
            {
                CorrelationId = props.CorrelationId,
            };
            
            var body = ea.Body.ToArray();
            var document = JsonDocument.Parse(Encoding.UTF8.GetString(body));
            document.RootElement.TryGetProperty("content", out var content);
            var contentStream = new MemoryStream(Convert.FromBase64String(content.ToString()));
            
            IChannel channel = cons.Channel;
            var documentContent = await requestHandler(contentStream);
            var responseBytes = Encoding.UTF8.GetBytes(documentContent);
            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey:props.ReplyTo,
                mandatory:true,
                basicProperties:replyProps,
                body:responseBytes);
            await _channel.BasicAckAsync(deliveryTag:ea.DeliveryTag, multiple:false);
        };
        
        await _channel.BasicConsumeAsync(_queueName, false, consumer);
    }
    
    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _channel.DisposeAsync();
    }
}