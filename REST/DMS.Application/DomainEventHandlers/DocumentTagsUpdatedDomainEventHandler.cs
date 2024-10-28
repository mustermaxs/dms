using DMS.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace DMS.Application.DomainEventHandlers;

public class DocumentTagsUpdatedDomainEventHandler(
    ILogger<DocumentTagsUpdatedDomainEvent> logger
) : IDomainEventHandler<DocumentTagsUpdatedDomainEvent>(logger)
{
    public override async Task HandleEvent(DocumentTagsUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;   
    }
}