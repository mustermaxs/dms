using MediatR;

namespace DMS.Domain.DomainEvents
{
    public abstract class DomainEvent : IRequest
    {
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    }
}