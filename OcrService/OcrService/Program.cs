
using WorkerService1;

namespace OcrService;
class Program
{
    private static RabbitMqServer _rabbitMq = new RabbitMqServer("localhost", "ocr");
    private static OcrWorker _ocrWorker = new OcrWorker();
    
    static async Task Main(string[] args)
    {
        await _rabbitMq.StartListeningAsync(async (pdfStream) =>
        {
            return await _ocrWorker.ProcessPdfAsync(pdfStream);
        });
        
        await Task.Delay(-1);
        await _rabbitMq.DisposeAsync();
    }
}