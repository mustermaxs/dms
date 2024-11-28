using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.IntegrationEvents
{
    public record ProcessingDocumentFailedIntegrationEvent(OcrProcessedDocumentDto OcrProcessedDocumentDto)
        : INotification;

    public class ProcessingDocumentFailedIntegrationEventHandler(
        ILogger<ProcessingDocumentFailedIntegrationEvent> logger,
        IUnitOfWork unitOfWork,
        IFileStorage fileStorage,
        IDmsDocumentRepository documentRepository)
        : Domain.DomainEvents.EventHandler<ProcessingDocumentFailedIntegrationEvent>(logger)
    {
        public override async Task HandleEvent(ProcessingDocumentFailedIntegrationEvent notification,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync();
            await unitOfWork.DmsDocumentRepository.DeleteById(notification.OcrProcessedDocumentDto.Id);
            await fileStorage.DeleteFileAsync(notification.OcrProcessedDocumentDto.Id);
            await unitOfWork.CommitAsync();
        }
    }
}