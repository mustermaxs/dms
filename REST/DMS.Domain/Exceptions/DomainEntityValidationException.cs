using DMS.Domain.Entities;

namespace DMS.Domain.Exceptions;

public class DomainEntityValidationException : Exception
{
    public readonly object Entity;
    public DomainEntityValidationException(string message, object entity, Exception innerException) : base(message, innerException)
    {
        Entity = entity;
    }
    public DomainEntityValidationException(Entity entity)
    {
        Entity = entity;
    }

    public DomainEntityValidationException(string message, in object entity) : base(message)
    {
        Entity = entity;
    }
}