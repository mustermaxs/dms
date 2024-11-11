using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;

namespace DMS.Infrastructure.Services;

public class OcrService(IMessageBroker messageBroker) : IOcrService
{
    private readonly IMessageBroker _messageBroker = messageBroker;
    public async Task<string> ProcessDocumentAsync(DmsDocument document)
    {
        await _messageBroker.StartAsync("ocr-process");
        return await messageBroker.PublishRpc<DmsDocument>("ocr-process", document);
    }

    // // TODO Implement
    // private async void SubscriptionHandler(string channel, BasicDeliverEventArgs eventArgs)
    // {
    //     var body = eventArgs.Body.ToArray();
    //     var message = Encoding.UTF8.GetString(body);
    //     
    // }
}