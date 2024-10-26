using DMS.Domain.Entities;
using DMS.Domain.Entities.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Domain.DomainEvents
{
    public record DocumentTagsUpdatedDomainEvent(DmsDocument Document) : INotification, IDomainEvent, IRequest;

    public class DocumentTagsUpdatedDomainEventHandler(
        ILogger<DocumentTagsUpdatedDomainEvent> logger
        ) : IDomainEventHandler<DocumentTagsUpdatedDomainEvent>(logger)
    {
        public override async Task HandleEvent(DocumentTagsUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;   
        }
    }
}

