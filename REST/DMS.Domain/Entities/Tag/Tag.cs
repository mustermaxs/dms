namespace DMS.Domain.Entities;

public class Tag : Entity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<DmsDocument> Documents { get; set; }

    public Tag CreateTag(string name)
    {
        return new Tag
        {
            Name = name
        };
    }
}