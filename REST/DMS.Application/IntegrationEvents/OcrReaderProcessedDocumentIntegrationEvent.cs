using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.IntegrationEvents
{
    public record OcrReaderProcessedDocumentIntegrationEvent(DmsDocument Document) : INotification;

    public class OcrReaderProcessedDocumentIntegrationEventHandler(
        ILogger<OcrReaderProcessedDocumentIntegrationEvent> logger,
        IUnitOfWork unitOfWork
    ) : Domain.DomainEvents.EventHandler<OcrReaderProcessedDocumentIntegrationEvent>(logger)
    {
        public override async Task HandleEvent(OcrReaderProcessedDocumentIntegrationEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                // TODO
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

