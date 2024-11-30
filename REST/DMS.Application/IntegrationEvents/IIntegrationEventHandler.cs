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
            try
            {
                var eventName = typeof(TEvent).FullName;
                logger.LogInformation($"[Integration Event] {notification}");
                await HandleEvent(notification, cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogCritical($" [Integration Event ERROR] {notification}. Exception: {e}");
                throw;
            }

        }

        public abstract Task HandleEvent(TEvent notification, CancellationToken cancellationToken);
    }
}
