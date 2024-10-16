using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;

namespace DMS.Domain.Entities.DomainEvents
{
    public record DocumentUploadedEvent(Guid Id, string Title) : IDomainEvent;
}