using DMS.Application.DTOs;

namespace DMS.Application.Interfaces;

public interface IOcrService
{
    Task ExtractTextFromPdfAsync(OcrDocumentRequestDto ocrDocumentRequest);
}