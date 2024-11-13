using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.ValueObjects;

namespace DMS.Application.DTOs;

public class DmsDocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime UploadDateTime { get; set; }
    public DateTime? ModificationDateTime { get; set; }
    public ProcessingStatus Status { get; set; }
    public List<TagDto>? Tags { get; set; } = new List<TagDto>();
    public FileType DocumentType { get; set; } = FileType.GetFileTypeFromExtension("null.pdf");
}