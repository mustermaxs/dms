using DMS.Application.DTOs;
using DMS.Application.EventHandlers;
using DMS.Application.Interfaces;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using DMS.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.IntegrationEvents
{
    public record DocumentSavedInFileStorageIntegrationEvent(DmsDocument Document) : INotification;

    public class DocumentSavedInFileStorageIntegrationEventHandler(
        ILogger<DocumentSavedInFileStorageIntegrationEvent> logger,
        IOcrService ocrService,
        IUnitOfWork unitOfWork,
        IDmsDocumentRepository documentRepository) : Domain.DomainEvents.EventHandler<DocumentSavedInFileStorageIntegrationEvent>(logger)
    {
        public override async Task HandleEvent(DocumentSavedInFileStorageIntegrationEvent notification,
            CancellationToken cancellationToken)
        {
            try
            {
                // TODO inform OCR Worker that file is ready to be processed
                // then, wait for OCR Worker to finish processing the file
                // then dispatch event that the file has been processed
                // handler should take care of updating content of document in db
                
                // await unitOfWork.BeginTransactionAsync();
                // notification.Document.SetStatus(ProcessingStatus.Pending);
                // await documentRepository.UpdateAsync(notification.Document);
                // await unitOfWork.CommitAsync();
                var document = await documentRepository.Get(notification.Document.Id);
                
                if (document is null)
                    throw new Exception($"Document with id {notification.Document.Id} not found");
                
                document.SetStatus(ProcessingStatus.Pending);
                await documentRepository.UpdateAsync(document);
                var tags = notification.Document.Tags?.ToList().Select(t => t.Tag.Label).ToList();
                await ocrService.ExtractTextFromPdfAsync(new OcrDocumentRequestDto(
                    notification.Document.Id,
                    tags,
                    notification.Document.Title)).ConfigureAwait(false);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                logger.LogError(e, $"[Event Error] {typeof(DocumentSavedInFileStorageIntegrationEvent).FullName}: {e.Message}");
                throw;
            }
        }
    }
}