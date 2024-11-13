using DMS.Application.Interfaces;
using DMS.Domain.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.IntegrationEvents
{
    public record ContentExtractedIntegrationEvent(Guid DocumentId, string Content) : INotification;
    
    public class ContentExtractedIntegrationEventHandler(
        ILogger<ContentExtractedIntegrationEvent> logger,
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IDmsDocumentRepository dmsDocumentRepository) : Domain.DomainEvents.EventHandler<ContentExtractedIntegrationEvent>(logger)
    {
        public override async Task HandleEvent(ContentExtractedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                var document = await dmsDocumentRepository.Get(notification.DocumentId);
                document.UpdateContent(notification.Content);
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