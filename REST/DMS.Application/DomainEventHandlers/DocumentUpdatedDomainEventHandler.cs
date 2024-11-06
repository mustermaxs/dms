using DMS.Application.Interfaces;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities.DomainEvents;
using Microsoft.Extensions.Logging;

namespace DMS.Application.DomainEventHandlers;

public class DocumentUpdatedDomainEventHandler(
    ILogger<DocumentUpdatedDomainEvent> logger,
    ISearchService searchService
) : Domain.DomainEvents.EventHandler<DocumentUpdatedDomainEvent>(logger)
{
    public override async Task HandleEvent(DocumentUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}