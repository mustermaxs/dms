using DMS.Domain.Entities;
using MediatR;

namespace DMS.Domain.DomainEvents;

public record DeletedDocumentInDbDomainEvent(DmsDocument Document) : INotification;