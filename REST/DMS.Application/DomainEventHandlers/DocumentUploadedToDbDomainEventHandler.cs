using DMS.Application.DTOs;
using DMS.Application.IntegrationEvents;
using DMS.Application.Interfaces;
using DMS.Application.Services;
using DMS.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.DomainEventHandlers;

public class DocumentUploadedToDbEventHandler(
    ILogger<DocumentUploadedToDbDomainEvent> logger,
    FileHelper fileHelper,
    IFileStorage fileStorage,
    IMediator mediator
    )
    : Domain.DomainEvents.EventHandler<DocumentUploadedToDbDomainEvent>(logger)
{ 
    public override async Task HandleEvent(DocumentUploadedToDbDomainEvent notification,
        CancellationToken cancellationToken)
    {
        try
        {
            var contentAsStream = fileHelper.FromBase64ToStream(notification.Content);
            await fileStorage.SaveFileAsync(notification.Document.Id, contentAsStream);
            // TODO Send Integration Event to notify that the document has been saved in the file storage
            await mediator.Publish(new DocumentSavedInFileStorageIntegrationEvent(notification.Document));
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await fileStorage.DeleteFileAsync(notification.Document.Id);
            mediator.Publish(new SaveDocumentInFsFailedEvent(notification.Document));
        }
    }
}