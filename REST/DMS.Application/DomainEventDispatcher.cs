using DMS.Application.Exceptions;
using DMS.Domain.Entities;
using DMS.Domain.Entities.DomainEvents;
using MediatR;

namespace DMS.Application
{
    public interface IEventDispatcher
    {
        Task DispatchEventsAsync(IReadOnlyCollection<object> entitiesWithEvents);
    }

    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchEventsAsync(IReadOnlyCollection<object> entitiesWithEvents)
        {
            foreach (var domainEvent in entitiesWithEvents)
            {
                var eventType = domainEvent.GetType();
                var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                var handler = _serviceProvider.GetService(handlerType);

                if (handler == null)
                {
                    throw new UploadDocumentException($"No Eventhandler for Domain Event {eventType.FullName} was found.");
                }
                
                await ((dynamic)handler).HandleAsync((dynamic)domainEvent);
            }
        }
    }
}