namespace DMS.Application.Interfaces
{
    public interface IMessageBroker
    {
        Task<string> PublishRpc<TMessageObject>(string queueName, TMessageObject messageObject);
        Task Subscribe(string queueName);
        Task Acknowledge(ulong deliveryTag);
        Task Reject(ulong deliveryTag, bool requeue);
        Task Close();
    }
}