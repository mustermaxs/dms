using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;

namespace DMS.Infrastructure.Services;

public class OcrService(IMessageBroker messageBroker) : IOcrService
{
    private readonly IMessageBroker _messageBroker = messageBroker;

    public async Task<string> ExtractTextFromPdfAsync(DmsDocumentDto document)
    {
        var content = await _messageBroker.PublishRpc<object>("ocr-doc", document);
        return content;
    }
}