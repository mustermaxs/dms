using DMS.Domain.Entities;
using DMS.Domain.Entities.DmsDocument;
using MediatR;

namespace DMS.Domain.DomainEvents;

public record DeletedDocumentDomainEvent(DmsDocument Document) : INotification;