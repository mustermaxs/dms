using DMS.Application.Interfaces;
using DMS.Domain.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.IntegrationEvents
{
    public record DocumentContentExtractedIntegrationEvent(Guid DocumentId, string Content) : INotification;

    public class DocumentContentExtractedIntegrationEventHandler(
        ILogger<DocumentContentExtractedIntegrationEvent> logger,
        IUnitOfWork unitOfWork,
        IDmsDocumentRepository documentRepository)
        : Domain.DomainEvents.EventHandler<DocumentContentExtractedIntegrationEvent>(logger)
    {
        public override async Task HandleEvent(DocumentContentExtractedIntegrationEvent notification,
            CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                var document = await documentRepository.Get(notification.DocumentId);
                if (document is null)
                {
                    throw new Exception($"Document with id {notification.DocumentId} not found");
                }
                logger.LogDebug($"Saving document content.\nId: {notification.DocumentId}.\nTitle: {document.Title}");
                document.UpdateContent(notification.Content);
                await unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"[Event Error] {typeof(DocumentContentExtractedIntegrationEvent).FullName}: {e.Message}");
                throw;
            }
        }
    }
}
