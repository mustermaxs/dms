using RabbitMQ.Client.Events;

namespace DMS.Application.Interfaces
{
    public interface IMessageBroker
    {
        Task Publish<TMessageObject>(string queueName, TMessageObject messageObject);
        Task Subscribe(string queueName, AsyncEventHandler<BasicDeliverEventArgs> eventHandler);
        Task Acknowledge(ulong deliveryTag);
        Task Reject(ulong deliveryTag, bool requeue);
        Task Close();
        Task StartAsync(string queueName);
    }
}