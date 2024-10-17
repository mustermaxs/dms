using DMS.Domain.Entities.DomainEvents;
using DMS.Infrastructure.Services;

namespace DMS.Infrastructure.EventHandlers;

public class DocumentCreatedEventHandler : IDomainEventHandler<DocumentSavedInDbEvent>
{
    private readonly IMessageBrokerClient _messageBrokerClient;

    public DocumentCreatedEventHandler(IMessageBrokerClient messageBrokerClient)
    {
        _messageBrokerClient = messageBrokerClient;
    }
    public Task HandleAsync(DocumentSavedInDbEvent domainEvent)
    {
        throw new NotImplementedException();
    }
}

