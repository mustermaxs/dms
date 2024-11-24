namespace DMS.Infrastructure.Models;

public class BasePersistenceModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
}