namespace DMS.Application.DTOs;

public record OcrDocumentRequestDto(Guid DocumentId, List<string>? Tags, string Title);