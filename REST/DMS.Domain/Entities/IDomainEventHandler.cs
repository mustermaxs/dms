namespace DMS.Domain.Entities.DomainEvents;

public interface IDomainEventHandler<in TDomainEvent>
{
    Task HandleAsync(TDomainEvent domainEvent);
}