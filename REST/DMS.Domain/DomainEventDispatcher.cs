using DMS.Domain.Entities;
using DMS.Domain.Entities.DomainEvents;

namespace DMS.Domain
{
    public interface IDomainEventDispatcher
    {
        Task DispatchEventsAsync(IReadOnlyCollection<object> entitiesWithEvents);
    }

    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchEventsAsync(IReadOnlyCollection<object> entitiesWithEvents)
        {
            foreach (var domainEvent in entitiesWithEvents)
            {
                var eventType = domainEvent.GetType();
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
                var handler = _serviceProvider.GetService(handlerType);

                if (handler != null)
                {
                    await ((dynamic)handler).HandleAsync((dynamic)domainEvent);
                }
            }
        }
    }
}