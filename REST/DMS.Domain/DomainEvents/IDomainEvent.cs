using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Domain.DomainEvents
{
public interface IDomainEvent : IRequest {}
    
public abstract class EventHandler<TEvent>(ILogger<TEvent> logger) : INotificationHandler<TEvent> where TEvent : INotification
{
    public ILogger Logger { get; } = logger;

    public async Task Handle(TEvent notification, CancellationToken cancellationToken)
    {
        var eventName = typeof(TEvent).FullName;
        logger.LogInformation($"[Domain Event] {notification}");
        await HandleEvent(notification, cancellationToken);
    }

    public abstract Task HandleEvent(TEvent notification, CancellationToken cancellationToken);
}
}

