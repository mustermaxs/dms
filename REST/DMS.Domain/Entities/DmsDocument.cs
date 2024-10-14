namespace DMS.Domain.Entities;

using DMS.Domain.ValueObjects;

public class DmsDocument
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public DateTime UploadDateTime { get; set; }
    public DateTime ModificationDateTime { get; set; }
    public string Path { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public FileType DocumentType { get; set; }
    public ProcessingStatus Status { get; set; }
}