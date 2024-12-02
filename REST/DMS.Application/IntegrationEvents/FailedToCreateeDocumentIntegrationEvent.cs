using DMS.Application.Commands;
using DMS.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.IntegrationEvents
{
    public record FailedToCreateeDocumentIntegrationEvent(UploadDocumentCommand Command) : INotification;
    
    public class FailedToCreateeDocumentIntegrationEventHandler(
        ILogger<FailedToCreateeDocumentIntegrationEvent> logger) : Domain.DomainEvents.EventHandler<FailedToCreateeDocumentIntegrationEvent>(logger)
    {
        public override async Task HandleEvent(FailedToCreateeDocumentIntegrationEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                logger.LogError(e, $"[Event Error] {typeof(FailedToCreateeDocumentIntegrationEvent).FullName}: {e.Message}");
                throw;
            }
        }
    }
}

