using DMS.Domain.ValueObjects;

namespace DMS.Application.DTOs;

public class DocumentSearchResultDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    // matching substring in document content/title
    public string Match { get; set; }
    public DateTime UploadDateTime { get; set; }
    public DateTime? ModificationDateTime { get; set; }
    public ProcessingStatus Status { get; set; }
    public List<TagDto> Tags { get; set; }
    public FileType DocumentType { get; set; }
}