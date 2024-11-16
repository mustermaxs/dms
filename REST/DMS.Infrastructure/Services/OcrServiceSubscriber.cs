using DMS.Application.DTOs;
using DMS.Application.IntegrationEvents;
using DMS.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DMS.Infrastructure.Services;
using Microsoft.Extensions.Logging;

public class OcrServiceSubscriber : IHostedService
{
    private Task _executingTask;
    private CancellationTokenSource _cts;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OcrServiceSubscriber(IServiceScopeFactory serviceScopeFactory)
    {
        _cts = new CancellationTokenSource();
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task ProcessOcrResults(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
        
        await messageBroker.Subscribe<OcrProcessedDocumentDto>("ocr-result", async (processedDocumentDto) =>
        {
            Console.WriteLine($"Received OCR result for document ID: {processedDocumentDto.Id}");
            using var callbackScope = _serviceScopeFactory.CreateScope();
            var mediator = callbackScope.ServiceProvider.GetRequiredService<IMediator>();
            var logger = callbackScope.ServiceProvider.GetRequiredService<ILogger<OcrServiceSubscriber>>();
            logger.LogInformation($"Received OCR result for document ID: {processedDocumentDto.Id}");
            
            await mediator.Publish(new DocumentContentExtractedIntegrationEvent(
                processedDocumentDto.Id,
                processedDocumentDto.Content), cancellationToken);
        });

        
        await Task.CompletedTask; 
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _executingTask = Task.Run(() => ProcessOcrResults(_cts.Token), cancellationToken);
        
        return Task.CompletedTask;
    }


    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        
        if (_executingTask != null)
        {
            try
            {
                await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
            }
            catch (OperationCanceledException)
            {
                
            }
        }
    }
    public void Dispose()
    {
        _cts?.Dispose();
    }
}