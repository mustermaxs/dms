using DMS.Domain.Entities;

namespace DMS.Domain.ValueObjects;

public class Tag
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<DmsDocument> Documents { get; set; }
}