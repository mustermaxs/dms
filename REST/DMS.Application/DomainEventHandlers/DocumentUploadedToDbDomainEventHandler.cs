using DMS.Application.DTOs;
using DMS.Application.IntegrationEvents;
using DMS.Application.Interfaces;
using DMS.Application.Services;
using DMS.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.DomainEventHandlers;

public class DocumentCreatedDomainEventHandler(
    ILogger<DocumentCreatedDomainEvent> logger,
    FileHelper fileHelper,
    IFileStorage fileStorage,
    IMediator mediator
    )
    : Domain.DomainEvents.EventHandler<DocumentCreatedDomainEvent>(logger)
{
    private readonly ILogger<DocumentCreatedDomainEvent> _logger = logger;

    public override async Task HandleEvent(DocumentCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Created document with id: {notification.Document.Id}");
            var contentAsStream = fileHelper.FromBase64ToStream(notification.Content);

            if (contentAsStream.Length <= 0)
            {
                throw new Exception($"CONTENT: {notification.Content}");
            }
            await fileStorage.SaveFileAsync(notification.Document.Id, contentAsStream);
            await mediator.Publish(new DocumentSavedInFileStorageIntegrationEvent(notification.Document));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            await fileStorage.DeleteFileAsync(notification.Document.Id);
        }
    }
}