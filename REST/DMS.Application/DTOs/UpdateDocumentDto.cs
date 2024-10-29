namespace DMS.Application.DTOs;

public class UpdateDocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public List<TagDto> Tags { get; set; }
}