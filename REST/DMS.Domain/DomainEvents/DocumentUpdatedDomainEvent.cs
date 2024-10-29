using DMS.Domain.Entities;
using DMS.Domain.Entities.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Domain.DomainEvents
{
    public record DocumentUpdatedDomainEvent(DmsDocument Document) : INotification, IDomainEvent, IRequest;
}

