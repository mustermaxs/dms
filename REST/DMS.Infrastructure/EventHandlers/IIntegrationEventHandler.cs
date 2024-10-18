using MediatR;

namespace DMS.Infrastructure.EventHandlers
{
    public interface IIntegrationEventHandler<in TDomainEvent>
    {
        Task HandleAsync(TDomainEvent domainEvent);
    }

    public interface IIntegrationEventDispatcher
    {
        Task DispatchEventsAsync(IReadOnlyCollection<object> entitiesWithEvents);
    }

    public class IntegrationEventDispatcher : IIntegrationEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public IntegrationEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchEventsAsync(IReadOnlyCollection<object> entitiesWithEvents)
        {
            foreach (var domainEvent in entitiesWithEvents)
            {
                var eventType = domainEvent.GetType();
                var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                var handler = _serviceProvider.GetService(handlerType);

                if (handler != null)
                {
                    await ((dynamic)handler).HandleAsync((dynamic)domainEvent);
                }
            }
        }
    }
}