using DMS.Api;
using DMS.Api.Configuration;
using DMS.Application.Interfaces;
using DMS.Application.Services;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;
using DMS.Infrastructure;
using DMS.Infrastructure.Repositories;
using DMS.Infrastructure.Services;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Testcontainers.Minio;

namespace DMS.Tests.DomainEvents.Mocks;

public class Givens
{
    public IMinioClient? _MinioClient { get; private set; } = null;
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
            var container  = new MinioBuilder()
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
            return container;
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

    public string GetMockBase64PdfContent()
    {
        var config = ServiceProvider.GetRequiredService<IConfiguration>();
        var mockBase64PdfContent = config["MockBase64PdfContent"];
        return mockBase64PdfContent ?? throw new Exception("MockBase64PdfContent not found in configuration");
    }
}