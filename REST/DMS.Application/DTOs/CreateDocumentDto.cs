namespace DMS.Application.DTOs;

public class CreateDocumentDto
{
    public string Title { get; set; }
    public DateTime UploadDateTime { get; set; }
    public List<TagDto> Tags { get; set; }
}