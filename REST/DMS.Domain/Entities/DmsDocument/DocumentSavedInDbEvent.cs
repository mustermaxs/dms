using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;

namespace DMS.Domain.Entities.DomainEvents
{
    public record DocumentSavedInDbEvent(Guid Id, string Title) : IDomainEvent;
    
    public class DocumentSavedInDbEventHandler : IDomainEventHandler<DocumentSavedInDbEvent>
    {
        public async Task HandleAsync(DocumentSavedInDbEvent domainEvent)
        {
            Console.WriteLine("Document uploaded: " + domainEvent.Title);
        }
    }
}