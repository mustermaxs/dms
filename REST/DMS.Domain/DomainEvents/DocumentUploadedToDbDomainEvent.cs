using DMS.Domain.Entities;
using DMS.Domain.Entities.Documents;
using MediatR;

namespace DMS.Domain.DomainEvents;

public record DocumentUploadedToDbDomainEvent(DmsDocument Document, string Content): INotification, IDomainEvent;