using DMS.Domain.DomainEvents;

namespace DMS.Domain.Entities.DomainEvents
{
    public interface IEventHandler<in TDomainEvent>
    {
        Task HandleAsync(TDomainEvent domainEvent);
    }
}