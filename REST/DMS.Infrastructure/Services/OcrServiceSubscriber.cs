using DMS.Application.DTOs;
using DMS.Application.IntegrationEvents;
using DMS.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

public class OcrServiceSubscriber : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OcrServiceSubscriber(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<OcrServiceSubscriber>>();

        logger.LogInformation("OCR service subscriber started.");
        await messageBroker.StartAsync("ocr-result");
        await messageBroker.Subscribe<OcrProcessedDocumentDto>("ocr-result", async (processedDocumentDto) =>
        {
            try
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation("Subscription cancelled.");
                    return;
                }
        
                using var callbackScope = _serviceScopeFactory.CreateScope();
                var mediator = callbackScope.ServiceProvider.GetRequiredService<IMediator>();

                logger.LogInformation($"Received OCR result for document ID: {processedDocumentDto.Id}");
                await mediator.Publish(new DocumentContentExtractedIntegrationEvent(
                    processedDocumentDto.Id,
                    processedDocumentDto.Content), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

        });

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("STOPPING OCR SERVICE SUBSCRIBER");
        return base.StopAsync(cancellationToken);
    }
}
