using DMS.Application.Interfaces;

namespace DMS.Infrastructure.Services;

public class OcrService(IMessageBroker messageBroker) : IOcrService
{
    private readonly IMessageBroker _messageBroker = messageBroker;

    public async Task<string> ExtractTextFromPdfAsync(string filePath)
    {
        return await Task.FromResult("Hello World");
    }
}