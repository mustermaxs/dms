namespace DMS.Infrastructure.Services
{
    public interface IMessageBrokerClient   // TODO wo soll das Interface hin? In Application Layer?
                                            // Implementierung bleibt in Infrastructure
    {
    }

    public class RabbitMqClient : IMessageBrokerClient
    {
        
    }
}