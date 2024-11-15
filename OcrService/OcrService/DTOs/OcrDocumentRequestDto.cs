using System.ComponentModel.DataAnnotations;

namespace OcrService.DTOs;

public class OcrDocumentRequestDto
{
    [Required]
    public Guid DocumentId { get; set; }
    [Required]
    public string Title { get; set; }
    public List<string>? Tags { get; set; }
}