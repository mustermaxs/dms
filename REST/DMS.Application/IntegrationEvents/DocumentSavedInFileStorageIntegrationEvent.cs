using AutoMapper;
using DMS.Application.DTOs;
using DMS.Application.EventHandlers;
using DMS.Application.Interfaces;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.IntegrationEvents
{
    public record DocumentSavedInFileStorageIntegrationEvent(DmsDocument Document) : INotification;

    public class DocumentSavedInFileStorageIntegrationEventHandler(
        ILogger<DocumentSavedInFileStorageIntegrationEvent> logger,
        IOcrService ocrService,
        IMapper mapper,
        IUnitOfWork unitOfWork) : Domain.DomainEvents.EventHandler<DocumentSavedInFileStorageIntegrationEvent>(logger)
    {
        public override async Task HandleEvent(
            DocumentSavedInFileStorageIntegrationEvent notification,
            CancellationToken cancellationToken)
        {
            try
            {
                // TODO inform OCR Worker that file is ready to be processed
                // then, wait for OCR Worker to finish processing the file
                // then dispatch event that the file has been processed
                // handler should take care of updating content of document in db
                var documentContent = await ocrService.ProcessDocumentAsync(notification.Document);
                // TODO how to document content?
                
                await unitOfWork.BeginTransactionAsync();
                
                notification.Document.UpdateContent(documentContent);
                await unitOfWork.DmsDocumentRepository.UpdateAsync(notification.Document);
                await unitOfWork.CommitAsync();
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}