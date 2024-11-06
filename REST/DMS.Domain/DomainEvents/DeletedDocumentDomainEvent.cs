using DMS.Domain.Entities;
using MediatR;

namespace DMS.Domain.DomainEvents;

public record DeletedDocumentDomainEvent(DmsDocument Document) : INotification;