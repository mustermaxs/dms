using DMS.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace DMS.Application.DomainEventHandlers;

public class DocumentTagsUpdatedEventHandler(
    ILogger<DocumentTagsUpdatedDomainEvent> logger
) : Domain.DomainEvents.EventHandler<DocumentTagsUpdatedDomainEvent>(logger)
{
    public override async Task HandleEvent(DocumentTagsUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;   
    }
}