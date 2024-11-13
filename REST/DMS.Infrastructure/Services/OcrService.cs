using System.Text;
using System.Text.Json;
using DMS.Application.DTOs;
using DMS.Application.IntegrationEvents;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Infrastructure.DTOs;
using MediatR;

namespace DMS.Infrastructure.Services;

public class OcrService(
    IMessageBroker messageBroker,
    IMediator mediator) : IOcrService
{
    private readonly IMessageBroker _messageBroker = messageBroker;
    private readonly IMediator _mediator = mediator;
    public Task ProcessDocumentAsync(DmsDocumentDto document)
    {
        _messageBroker.StartAsync("ocr-process");
        messageBroker.Publish<OcrDocumentRequestDto>(
            "ocr-process",
            new OcrDocumentRequestDto
            {
                Id = document.Id,
                Tags = document.Tags?.Select(t => t.Label).ToList(),
                Title = document.Title
            });

        _messageBroker.Subscribe("ocr-result", async (channel, args) =>
        {
            byte[] body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var ocrDocumentDto = JsonSerializer.Deserialize<OcrProcessedDocumentDto>(message);
            await mediator.Publish(new ContentExtractedIntegrationEvent(ocrDocumentDto.Id, ocrDocumentDto.Content));
        });

        return Task.CompletedTask;
    }
}