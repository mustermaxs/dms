using DMS.Domain.Entities.DomainEvents;

namespace DMS.Domain.Entities;

using DMS.Domain.ValueObjects;

public class DmsDocument : AggregateRoot
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public DateTime UploadDateTime { get; set; }
    public DateTime? ModificationDateTime { get; set; }
    public string? Path { get; set; }
    public List<Tag> Tags { get; set; }
    public FileType DocumentType { get; set; }
    public ProcessingStatus Status { get; set; }

    public DmsDocument CreateDocument(string title)
    {
        var document = new DmsDocument
        {
            Id = Guid.NewGuid(),
            UploadDateTime = DateTime.Now,
            ModificationDateTime = null,
            Path = String.Empty,
            Content = String.Empty,
            Title = title,
            Status = ProcessingStatus.Pending
        };
        
        AddDomainEvent(new DocumentUploadedEvent(document.Id, document.Title));

        return document;
    }

    public void AddTag(Tag tag)
    {
        if (Tags.Contains(tag))
        {
            return;
        }
        Tags.Add(tag);
    }

    public void RemoveTag(Tag tag)
    {
        Tags.Remove(tag);
    }
}