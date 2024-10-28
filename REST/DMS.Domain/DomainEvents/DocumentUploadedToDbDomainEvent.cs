using DMS.Domain.Entities;
using MediatR;

namespace DMS.Domain.DomainEvents;

public record DocumentUploadedToDbDomainEvent(DmsDocument Document): INotification, IDomainEvent;