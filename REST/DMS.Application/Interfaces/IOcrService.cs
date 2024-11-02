namespace DMS.Application.Interfaces;

public interface IOcrService
{
    Task<string> ExtractTextFromPdfAsync(string filePath);
}