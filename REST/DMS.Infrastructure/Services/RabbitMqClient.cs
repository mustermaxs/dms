using DMS.Application.Interfaces;

namespace DMS.Infrastructure.Services
{
    public class RabbitMqClient : IMessageBroker
    {
        public async Task Publish(string exchange, string routingKey, string message)
        {
            throw new NotImplementedException();
        }

        public async Task Subscribe(string queueName, Action<string> handler)
        {
            throw new NotImplementedException();
        }

        public async Task Acknowledge(ulong deliveryTag)
        {
            throw new NotImplementedException();
        }

        public async Task Reject(ulong deliveryTag, bool requeue)
        {
            throw new NotImplementedException();
        }

        public async Task Close()
        {
            throw new NotImplementedException();
        }
    }
}