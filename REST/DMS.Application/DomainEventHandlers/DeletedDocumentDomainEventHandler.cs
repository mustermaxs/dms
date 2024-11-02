using DMS.Application.Interfaces;
using DMS.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace DMS.Application.DomainEventHandlers;

public class DeletedDocumentDomainEventHandler(
    ILogger<DeletedDocumentDomainEvent> logger,
    IFileStorage fileStorage
    ) : Domain.DomainEvents.EventHandler<DeletedDocumentDomainEvent>(logger)
{
    public override async Task HandleEvent(DeletedDocumentDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await fileStorage.DeleteFileAsync(notification.Document.Id);
            // TODO Delete file from ElasticSearch
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete file from storage");
            throw;
        }    
    }
}