using DMS.Domain.Entities;
using MediatR;

namespace DMS.Domain.DomainEvents;

public record DocumentCreatedDomainEvent(DmsDocument Document, string Content): INotification, IDomainEvent;