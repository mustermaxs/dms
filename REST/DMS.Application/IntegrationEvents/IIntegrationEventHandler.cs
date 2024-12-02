using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.EventHandlers
{
    public interface IIntegrationEventHandler<in TDomainEvent>
    {
        Task HandleAsync(TDomainEvent domainEvent);
    }

    public abstract class IntegrationEventHandler<TEvent>(ILogger<TEvent> logger)
        : INotificationHandler<TEvent> where TEvent : INotification
    {
        public ILogger Logger { get; } = logger;

        public async Task Handle(TEvent notification, CancellationToken cancellationToken)
        {
            var eventName = typeof(TEvent).FullName;
            logger.LogInformation($"[Integration Event] {eventName}: {notification}");
            await HandleEvent(notification, cancellationToken);
        }

        public abstract Task HandleEvent(TEvent notification, CancellationToken cancellationToken);
    }
}
