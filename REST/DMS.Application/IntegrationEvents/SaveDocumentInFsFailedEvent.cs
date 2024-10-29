using DMS.Application.Interfaces;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.IntegrationEvents
{
public record SaveDocumentInFsFailedEvent(DmsDocument Document) : INotification;

public class SaveDocumentInFsFailedEventHandler(
    ILogger<SaveDocumentInFsFailedEvent> logger,
    IUnitOfWork unitOfWork) : IDomainEventHandler<SaveDocumentInFsFailedEvent>(logger)
{
    public override async Task HandleEvent(SaveDocumentInFsFailedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();
            await unitOfWork.DmsDocumentRepository.DeleteById(notification.Document.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
}

