
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Minio;
using OcrService.DTOs;
using WorkerService1;

namespace OcrService;
class Program
{
    private static IMinioClient _minioClient;
    private static MinioConfig _minioConfig;
    private static RabbitMqClient? _rabbitMq = null;
    private static OcrWorker _ocrWorker = new OcrWorker();
    private static RabbitMqConfig _rabbitMqConfig;
    private static IConfigurationRoot _configurationBuilder;
    private static FileStorage _fileStorage;
    static async Task Main(string[] args)
    {
        LoadConfig(Directory.GetCurrentDirectory());
        _rabbitMqConfig = _configurationBuilder.GetSection("RabbitMq").Get<RabbitMqConfig>();
        _minioConfig = _configurationBuilder.GetSection("Minio").Get<MinioConfig>();
        _minioClient = new MinioClient()
            .WithEndpoint(_rabbitMqConfig.Endpoint)
            .WithCredentials(_minioConfig.AccessKey, _minioConfig.SecretKey);
        _fileStorage = new FileStorage(_minioClient, _minioConfig);
        _rabbitMq = new RabbitMqClient(_rabbitMqConfig);
        
        await _rabbitMq.InitiliazeAsync();
        
        await _rabbitMq.Subscribe("ocr-process", async (model, ea) =>
        {
            try
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var ocrDocumentRequestDto = JsonSerializer.Deserialize<OcrDocumentRequestDto>(message);
                var fileStream = await _fileStorage.GetFileStreamAsync(ocrDocumentRequestDto.Id.ToString());
                var fileContent = await _ocrWorker.ProcessPdfAsync(fileStream);
                var ocrDocumentDto = new OcrProcessedDocumentDto { Content = fileContent, Id = ocrDocumentRequestDto.Id };
                await _rabbitMq.Publish<OcrProcessedDocumentDto>("ocr-result", ocrDocumentDto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        });
        
        await Task.Delay(-1);
        _rabbitMq.Dispose();
    }
    private static void LoadConfig(string path)
    {
        _configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}