using System.Text;
using System.Text.Json;
using DMS.Application.DTOs;
using DMS.Application.Interfaces;

namespace DMS.Infrastructure.Services;

public class OcrService(IMessageBroker messageBroker) : IOcrService
{
    private readonly IMessageBroker _messageBroker = messageBroker;
    public async Task ExtractTextFromPdfAsync(OcrDocumentRequestDto ocrDocumentRequest)
    {
        await _messageBroker.Publish<OcrDocumentRequestDto>("ocr-process", ocrDocumentRequest).ConfigureAwait(false);
        await _messageBroker.Subscribe<OcrProcessedDocumentDto>("ocr-result", async (OcrProcessedDocumentDto processedDocumentDto) =>
        {
            Console.WriteLine(processedDocumentDto.Content);
        }).ConfigureAwait(false);
    }
}