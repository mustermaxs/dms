using DMS.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace DMS.Application.DomainEventHandlers;

public class DocumentUpdatedDomainEventHandler(
    ILogger<DocumentUpdatedDomainEvent> logger
) : IDomainEventHandler<DocumentUpdatedDomainEvent>(logger)
{
    public override async Task HandleEvent(DocumentUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;   
    }
}