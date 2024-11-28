﻿using System.Reflection;
using System.Text;
using System.Text.Json;
using IronOcr;
using Microsoft.Extensions.Configuration;
using Minio;
using OcrService.DTOs;
using WorkerService1;
using OcrService.Configs;

namespace OcrService;
class Program
{
    private static IMinioClient _minioClient;
    private static FileStorageConfig _fileStorageConfig;
    private static RabbitMqClient? _rabbitMq = null;
    private static OcrWorker _ocrWorker = new OcrWorker();
    private static RabbitMqConfig _rabbitMqConfig;
    private static IConfigurationRoot _configurationBuilder;
    private static FileStorage _fileStorage;
    private static ElasticSearchService _elasticSearch;
    private static ElasticSearchConfig _elasticSearchConfig;

    static async Task Main(string[] args)
    {
        License.LicenseKey = "IRONSUITE.IF22B066.TECHNIKUM.WIEN.AT.13212-E87A0AF9CC-AIQRPGWZD57RN3RC-P3SK7TQNFL23-LUGKCGFU42LV-CELNTVRC7Y5B-GQKQEZYCMJ3H-AMVEAWUIPLZU-UDGLQC-TVSAPLTXUJWOEA-DEPLOYMENT.TRIAL-PGRG5C.TRIAL.EXPIRES.16.DEC.2024";
        LoadConfig(Directory.GetCurrentDirectory());
        _rabbitMqConfig = _configurationBuilder.GetSection("RabbitMq").Get<RabbitMqConfig>();
        _fileStorageConfig = _configurationBuilder.GetSection("MinIO").Get<FileStorageConfig>();
        _minioClient = new MinioClient()
            .WithEndpoint(_fileStorageConfig.Endpoint)
            .WithCredentials(_fileStorageConfig.AccessKey, _fileStorageConfig.SecretKey)
            .Build();
        _fileStorage = new FileStorage(_minioClient, _fileStorageConfig);
        _rabbitMq = new RabbitMqClient(_rabbitMqConfig);

        _elasticSearchConfig = _configurationBuilder.GetSection("ElasticSearch").Get<ElasticSearchConfig>();

        _elasticSearch = new ElasticSearchService(_elasticSearchConfig);
        
        await _rabbitMq.InitiliazeAsync();
        await _rabbitMq.Subscribe("ocr-process", async (model, ea) =>
        {
            Guid documentId = Guid.Empty;
            
            try
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var ocrDocumentRequestDto = JsonSerializer.Deserialize<OcrDocumentRequestDto>(message);
                documentId = ocrDocumentRequestDto.DocumentId;
                Console.WriteLine($"{DateTime.UtcNow} [x] Request to process {ocrDocumentRequestDto.DocumentId}");
                var fileStream = await _fileStorage.GetFileStreamAsync(ocrDocumentRequestDto.DocumentId.ToString());
                var fileContent = await _ocrWorker.ProcessPdfAsync(fileStream);

                await _elasticSearch.IndexDocumentAsync(
                    ocrDocumentRequestDto.DocumentId, 
                    fileContent, 
                    ocrDocumentRequestDto.Title, 
                    ocrDocumentRequestDto.Tags ?? new List<string>()
                );

                var ocrDocumentDto = new OcrProcessedDocumentDto { Content = fileContent, Id = ocrDocumentRequestDto.DocumentId, Status = ProcessStatus.Succeeded};

                await _rabbitMq.Publish<OcrProcessedDocumentDto>("ocr-result", ocrDocumentDto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // TODO publish to queue "ocr-failed" or easier:
                // publish to ocr-result but indicate in DTO somehow that it failed
                var ocrDocumentDto = new OcrProcessedDocumentDto { Status = ProcessStatus.Failed, Content = String.Empty, Id = documentId };
                await _rabbitMq.Publish<OcrProcessedDocumentDto>("ocr-result", ocrDocumentDto);
                throw new Exception($"Error processing message: {e.Message}");
            }
        });
        
        await Task.Delay(-1);
        _rabbitMq.Dispose();
    }
    private static void LoadConfig(string path)
    {
        _configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}