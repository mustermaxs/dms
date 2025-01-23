using System.Reflection;
using System.Text;
using System.Text.Json;
using IronOcr;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Minio;
using OcrService.DTOs;
using WorkerService1;
using OcrService.Configs;
using log4net.Config;
using System.IO;
using System.Reflection;
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
    private static readonly string IronOcrApiKey;
    public static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


    static async Task Main(string[] args)
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        InitializeLog4Net();
        Program.logger.Info("OCR Service started");
        await SetupServices();
        await _rabbitMq.Subscribe("ocr-process", async (model, ea) =>
        {
            Guid documentId = Guid.Empty;
            try
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var ocrDocumentRequestDto = JsonSerializer.Deserialize<OcrDocumentRequestDto>(message);
                documentId = ocrDocumentRequestDto.DocumentId;
                logger.Info($"Received request to process document {ocrDocumentRequestDto.DocumentId}");
                var fileStream = await _fileStorage.GetFileStreamAsync(ocrDocumentRequestDto.DocumentId.ToString());
                var fileContent = await _ocrWorker.ProcessPdfAsync(fileStream);

                await _elasticSearch.IndexDocumentAsync(
                    ocrDocumentRequestDto.DocumentId,
                    fileContent,
                    ocrDocumentRequestDto.Title,
                    ocrDocumentRequestDto.Tags ?? new List<string>()
                );

                var ocrDocumentDto = new OcrProcessedDocumentDto
                    { Content = fileContent, Id = ocrDocumentRequestDto.DocumentId, Status = ProcessStatus.Succeeded };

                await _rabbitMq.Publish<OcrProcessedDocumentDto>("ocr-result", ocrDocumentDto);
                logger.Info($"Document {ocrDocumentRequestDto.DocumentId} processed successfully");
            }
            catch (Exception e)
            {
                var errorMessage = $"Error processing document {documentId}: {e.Message}";
                if (e.InnerException != null)
                {
                    errorMessage += $" Inner exception: {e.InnerException.Message}";
                }

                logger.Error($"Processing document {documentId} failed! {errorMessage}. {e.StackTrace}");
                ;

                var ocrDocumentDto = new OcrProcessedDocumentDto
                {
                    Status = ProcessStatus.Failed,
                    Content = String.Empty,
                    Id = documentId,
                };
                await _rabbitMq.Publish<OcrProcessedDocumentDto>("ocr-result", ocrDocumentDto);
                logger.Info($"Publish failed processing attempt info for document {documentId}");
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

    private static async Task SetupServices()
    {
        logger.Info($"Loading configuration...");
        LoadConfig(Directory.GetCurrentDirectory());
        License.LicenseKey = _configurationBuilder.GetSection("Ocr:ApiKey").Value;
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
        logger.Info($"Configuration loaded.");
        await _rabbitMq.InitiliazeAsync();
        logger.Info($"RabbitMQ initialized.");
    }

    private static void InitializeLog4Net()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        var log4netConfigFile = new FileInfo("log4net.config");

        if (!log4netConfigFile.Exists)
        {
            Console.Error.WriteLine("log4net configuration file not found.");
            throw new FileNotFoundException("log4net configuration file not found.", log4netConfigFile.FullName);
        }

        XmlConfigurator.Configure(logRepository, log4netConfigFile);
        logger.Info("log4net has been configured.");
    }
}