using DMS.Application.DTOs;
using DMS.Application.EventHandlers;
using DMS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.IntegrationEvents
{
    public record DocumentSavedInFileStorageIntegrationEvent(DmsDocument document) : INotification;
    
    public class DocumentSavedInFileStorageIntegrationEventHandler(
        ILogger<DocumentSavedInFileStorageIntegrationEvent> logger
        ) : IntegrationEventHandler<DocumentSavedInFileStorageIntegrationEvent>(logger)
    {
        public override async Task HandleEvent(DocumentSavedInFileStorageIntegrationEvent notification, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}