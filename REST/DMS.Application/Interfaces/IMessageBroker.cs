
namespace DMS.Application.Interfaces
{
    public interface IMessageBroker
    {
        Task Publish<TMsgObject>(string queueName, TMsgObject msgObject);
        Task Subscribe<TResponseObj>(string queueName, Action<TResponseObj> callback);
        Task Acknowledge(ulong deliveryTag);
        Task Reject(ulong deliveryTag, bool requeue);
        Task Close();
        public Task EnsureInitialized();
        public Task<bool> QueueExists(string queueName);
        public Task StartAsync(string queueName);
    }
}