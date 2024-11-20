using DMS.Application.Interfaces;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using DMS.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.IntegrationEvents
{
public record SaveDocumentInFsFailedEvent(DmsDocument Document) : INotification;

public class SaveDocumentInFsFailedEventHandler(
    ILogger<SaveDocumentInFsFailedEvent> logger,
    IUnitOfWork unitOfWork,
    IDmsDocumentRepository documentRepository) : Domain.DomainEvents.EventHandler<SaveDocumentInFsFailedEvent>(logger)
{
    public override async Task HandleEvent(SaveDocumentInFsFailedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();
            notification.Document.SetStatus(ProcessingStatus.Failed);
            await unitOfWork.DmsDocumentRepository.UpdateAsync(notification.Document);
            await unitOfWork.CommitAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
}

