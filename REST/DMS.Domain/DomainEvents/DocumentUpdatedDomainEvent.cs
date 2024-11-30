using DMS.Domain.Entities.Documents;
using MediatR;

namespace DMS.Domain.DomainEvents
{
    public record DocumentUpdatedDomainEvent(DmsDocument Document) : INotification, IDomainEvent, IRequest;
}

