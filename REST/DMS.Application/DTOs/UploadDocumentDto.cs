using DMS.Domain.Entities.Tag;

namespace DMS.Application.DTOs;

public class UploadDocumentDto
{
    public string Title { get; set; }
    // Content in Base64 string format
    public string Content { get; set; }
    public List<TagDto> Tags { get; set; }
}