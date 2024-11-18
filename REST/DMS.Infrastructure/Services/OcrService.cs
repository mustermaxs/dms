using System.Text;
using System.Text.Json;
using DMS.Application.DTOs;
using DMS.Application.IntegrationEvents;
using DMS.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DMS.Infrastructure.Services;

public class OcrService(
    IMessageBroker messageBroker,
    IServiceScopeFactory serviceScopeFactory) : IOcrService
{
    private readonly IMessageBroker _messageBroker = messageBroker;

    public async Task ExtractTextFromPdfAsync(OcrDocumentRequestDto ocrDocumentRequest)
    {
        await _messageBroker.Publish<OcrDocumentRequestDto>("ocr-process", ocrDocumentRequest).ConfigureAwait(false);
    }
}