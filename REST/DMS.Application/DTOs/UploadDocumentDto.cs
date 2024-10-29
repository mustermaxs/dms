using DMS.Domain.Entities.Tag;
using Microsoft.AspNetCore.Http;

namespace DMS.Application.DTOs;

public class UploadDocumentDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public List<TagDto> Tags { get; set; }
}