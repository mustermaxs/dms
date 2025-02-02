namespace DMS.Domain.Entities;

public class Entity
{
    protected List<object> _domainEvents = new List<object>();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();
    public Guid Id { get; set; } = Guid.NewGuid();

    public void AddDomainEvent(object eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void AddDomainEventIfNotExists(object eventItem)
    {
        if (!_domainEvents.Contains(eventItem))
        {
            _domainEvents.Add(eventItem);
        }
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}