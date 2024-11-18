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
    public record DocumentIndexedIntegrationEvent(DmsDocument Document) : INotification;

    public class DocumentIndexedIntegrationEventHandler(
        ILogger<DocumentIndexedIntegrationEvent> logger,
        IOcrService ocrService,
        IUnitOfWork unitOfWork,
        IDmsDocumentRepository documentRepository) : Domain.DomainEvents.EventHandler<DocumentIndexedIntegrationEvent>(logger)
    {
        public override async Task HandleEvent(DocumentIndexedIntegrationEvent notification,
            CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(200, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}