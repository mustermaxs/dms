namespace DMS.Application.Interfaces
{
    public interface IMessageBroker
    {
        Task Publish(string exchange, string routingKey, string message);
        Task Subscribe(string queueName, Action<string> handler);
        Task Acknowledge(ulong deliveryTag);
        Task Reject(ulong deliveryTag, bool requeue);
        Task Close();
    }
}