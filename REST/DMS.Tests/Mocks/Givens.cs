using DMS.Api;
using DMS.Api.Configuration;
using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Application.Services;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;
using DMS.Domain.ValueObjects;
using DMS.Infrastructure;
using DMS.Infrastructure.Repositories;
using DMS.Infrastructure.Services;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Testcontainers.Minio;
using Moq;

namespace DMS.Tests.DomainEvents.Mocks;

public class Givens : IAsyncDisposable
{
    public readonly Mock<IMediator> _MockMediator;
    public IMinioClient? _MinioClient { get; private set; } = null;
    public IContainer? _MinioContainer { get; private set; } = null;
    public ServiceProvider ServiceProvider { get; private set; }
    private DmsDbContext? _dbContext { get; set; } = null;

    public DmsDbContext DbContext
    {
        get
        {
            if (_dbContext is null)
            {
                _dbContext = ServiceProvider.GetRequiredService<DmsDbContext>();
            }

            return _dbContext;
        }
        private set => _dbContext = value;
    }

    public Givens()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddDbContext<DmsDbContext>(options => options.UseInMemoryDatabase("DmsDbContext"));
        services.AddAutoMapper( typeof(DmsMappingProfile) );
        // REPOSITORIES
        services.AddScoped<IDmsDocumentRepository, DmsDocumentRepository>();
        services.AddScoped<IDocumentTagRepository, DocumentTagRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

// VALIDATORS
        services.AddScoped<IValidator<DmsDocument>, DmsDocumentValidator>();
        services.AddScoped<IValidator<Tag>, TagValidator>();
        services.AddScoped<IValidator<DocumentTag>, DocumentTagValidator>();
        
        services.AddSingleton<IMinioClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var minioConfig = config.GetSection("MinIO").Get<DmsMinioConfig>();

            return new MinioClient()
                .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
                .WithEndpoint(minioConfig.Endpoint)
                .Build();
        });

        services.AddScoped<IDmsDocumentRepository, DmsDocumentRepository>();
        services.AddScoped<IDocumentTagRepository, DocumentTagRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IDocumentTagFactory, DocumentTagFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<FileHelper>();


        services.AddScoped<IValidator<DmsDocument>, DmsDocumentValidator>();
        services.AddScoped<IValidator<Tag>, TagValidator>();
        services.AddScoped<IValidator<DocumentTag>, DocumentTagValidator>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public async Task<IContainer> GivenMinioContainer()
    {
        try
        {
            var minioConfig = GetMinioConfig();
            var container = new MinioBuilder()
                .WithImage("minio/minio")
                .WithPortBinding(9000)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9000))
                .WithEnvironment("MINIO_ACCESS_KEY", "minioadmin")
                .WithEnvironment("MINIO_SECRET_KEY", "minioadmin")
                .WithEnvironment("MINIO_ROOT_PASSWORD", "minioadmin")
                .WithEnvironment("MINIO_ROOT_USER", "minioadmin")
                .Build();

            container.StartAsync()
                .GetAwaiter()
                .GetResult();
            _MinioContainer = container;
            
            return _MinioContainer;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public IMinioClient GivenMinioClient()
    {
        if (_MinioClient is null)
        {
            var config = ServiceProvider.GetRequiredService<IConfiguration>();
            var minioConfig = config.GetSection("MinIO").Get<DmsMinioConfig>();

            _MinioClient = new MinioClient()
                .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
                .WithEndpoint("localhost:9000").Build();
        }

        return _MinioClient;
    }

    public DmsDbContext GetContext()
    {
        return ServiceProvider.GetRequiredService<DmsDbContext>();
    }

    public ITagRepository GivenTagRepository()
    {
        GivenDbContext();
        return ServiceProvider.GetService<ITagRepository>()!;
    }

    public IDmsDocumentRepository GivenDmsDocumentRepository()
    {
        GivenDbContext();
        return ServiceProvider.GetService<IDmsDocumentRepository>()!;
    }

    public async Task GivenAddTags(List<Tag> tags)
    {
        var context = GetContext();
        context.Tags.AddRange(tags);
        await context.SaveChangesAsync();
    }

    public async Task GivenSomeDocuments(List<DmsDocument> documents)
    {
        GivenDbContext();
        var context = GetContext();
        context.Documents.AddRange(documents);
        await context.SaveChangesAsync();
    }

    public void GivenDbContext()
    {
        var services = new ServiceCollection();
        services.AddDbContext<DmsDbContext>(options => options.UseInMemoryDatabase("DmsDbContext"));

        services.AddScoped<IDmsDocumentRepository, DmsDocumentRepository>();
        services.AddScoped<IDocumentTagRepository, DocumentTagRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        services.AddScoped<IValidator<DmsDocument>, DmsDocumentValidator>();
        services.AddScoped<IValidator<Tag>, TagValidator>();
        services.AddScoped<IValidator<DocumentTag>, DocumentTagValidator>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public IDocumentTagFactory GivenDocumentTagFactory()
    {
        GivenDbContext();
        return new DocumentTagFactory(
            ServiceProvider.GetRequiredService<ITagRepository>()
        );
    }

    public DmsMinioConfig GetMinioConfig()
    {
        var config = ServiceProvider.GetRequiredService<IConfiguration>();
        return config.GetSection("MinIO").Get<DmsMinioConfig>();
    }

    public IFileStorage GivenFileStorage(IMinioClient client)
    {
        return new FileStorage(client, ServiceProvider.GetRequiredService<IConfiguration>());
    }

    public string GivenPdfContentInBase64()
    {
        var config = ServiceProvider.GetRequiredService<IConfiguration>();
        var mockBase64PdfContent = config["MockBase64PdfContent"];
        return mockBase64PdfContent ?? throw new Exception("MockBase64PdfContent not found in configuration");
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContext != null)
        {
            _dbContext.Tags.RemoveRange(_dbContext.Tags);
            _dbContext.Documents.RemoveRange(_dbContext.Documents);
            await _dbContext.SaveChangesAsync();
            await _dbContext.DisposeAsync();
        }

        await DisposeMinioAsync();
        await ServiceProvider.DisposeAsync();
        if (_dbContext != null) await _dbContext.DisposeAsync();
    }

    private async Task DisposeMinioAsync()
    {
        var minioClientAsyncDisposable = _MinioClient as IAsyncDisposable;
        if (minioClientAsyncDisposable != null)
            await minioClientAsyncDisposable.DisposeAsync();
        else if (_MinioClient != null)
            _MinioClient.Dispose();
        
        if (_MinioContainer != null)
            await _MinioContainer.DisposeAsync();
    }

    public async Task<DmsDocument> GivenDocumentInDb(List<Tag>? tags = null)
    {
        var repo = ServiceProvider.GetRequiredService<IDmsDocumentRepository>();
        var document = DmsDocument.Create("testDocument.pdf",
            DateTime.UtcNow,
            "",
            new List<DocumentTag>
            {
            },
            new FileType("testDocument.pdf"),
            ProcessingStatus.NotStarted);
        
        tags?.ForEach(t => document.AddTag(t));
        await repo.Create(document);
        await repo.SaveAsync();

        return document;
    }

    public DmsDocument GivenDocument()
    {
        return DmsDocument.Create("testDocument.pdf",
            DateTime.UtcNow,
            "",
            new List<DocumentTag>
            {
            },
            new FileType("testDocument.pdf"),
            ProcessingStatus.NotStarted);
    }

    public async Task<DmsDocument> GivenDocumentInDbAndFileStorage()
    {
        if (_MinioContainer == null)
        {
            await GivenMinioContainer();
        }

        if (_MinioClient == null)
        {
            GivenMinioClient();
        }
        
        var document = await GivenDocumentInDb();
        var fileStorage = GivenFileStorage(_MinioClient);
        var fileHelper = ServiceProvider.GetRequiredService<FileHelper>();
        var mockContent = GivenPdfContentInBase64();
        await fileStorage.SaveFileAsync(document.Id, fileHelper.FromBase64ToStream(mockContent));
        
        return document;
    }
}