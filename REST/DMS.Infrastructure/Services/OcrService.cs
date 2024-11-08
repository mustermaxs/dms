using DMS.Application.DTOs;
using DMS.Application.Interfaces;

namespace DMS.Infrastructure.Services;

public class OcrService(IMessageBroker messageBroker) : IOcrService
{
    private readonly IMessageBroker _messageBroker = messageBroker;
    public async Task<string> ProcessDocumentAsync(DmsDocumentDto document)
    {
        return await messageBroker.PublishRpc<DmsDocumentDto>("ocr-process", document);
    }

    // // TODO Implement
    // private async void SubscriptionHandler(string channel, BasicDeliverEventArgs eventArgs)
    // {
    //     var body = eventArgs.Body.ToArray();
    //     var message = Encoding.UTF8.GetString(body);
    //     
    // }
}