using Minio;
using log4net;
using MediatR;
using log4net.Config;
using RabbitMQ.Client;
using FluentValidation;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using DMS.Api.Configuration;
using DMS.Application;
using DMS.Application.DTOs;
using DMS.Application.Services;
using DMS.Application.Commands;
using DMS.Application.Interfaces;
using DMS.Application.IntegrationEvents;
using DMS.Domain.Services;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.Entities.Tags;
using DMS.Domain.Entities.DmsDocument;
using DMS.Infrastructure;
using DMS.Infrastructure.Configs;
using DMS.Infrastructure.Services;
using DMS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// COMMANDS & QUERIES
builder.Services.AddMediatR(
    typeof(CreateTagCommand).Assembly,
    typeof(UploadDocumentCommand).Assembly,
    typeof(UpdateDocumentCommand).Assembly,
    typeof(DocumentSavedInFileStorageIntegrationEvent).Assembly,
    typeof(DocumentTagsUpdatedDomainEvent).Assembly,
    typeof(DocumentUpdatedDomainEvent).Assembly
    );

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AUTOMAPPER
builder.Services.AddAutoMapper(
    typeof(ApplicationMappingProfile),
    typeof(InfrastructureMappingProfile));

// LOGGING
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
builder.Logging.ClearProviders(); 
builder.Logging.AddLog4Net();  

// SERVICES
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITagCreateService, TagCreateService>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddTransient<FileHelper>();
builder.Services.AddScoped<IOcrService, OcrService>();
builder.Services.AddScoped<ISearchService, ElasticSearchService>();

// MINIO
builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var minioConfig = config.GetSection("MinIO").Get<DmsMinioConfig>();
    
    return new MinioClient()
        .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
        .WithEndpoint(minioConfig.Endpoint)
        .Build();
});

// RABBITMQ
builder.Services.AddScoped<IConnectionFactory, ConnectionFactory>();
builder.Services.AddSingleton<IMessageBroker, RabbitMqClient>(cfg =>
{
    var config = cfg.GetRequiredService<IConfiguration>();
    var rabbitMqConfig = config.GetSection("RabbitMq").Get<RabbitMqConfig>();
    return new RabbitMqClient(rabbitMqConfig!);
});

builder.Services.AddScoped<IFileStorage, FileStorage>();

// REPOSITORIES
builder.Services.AddScoped<IDmsDocumentRepository, DmsDocumentRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// VALIDATORS
builder.Services.AddScoped<IValidator<DmsDocument>, DmsDocumentValidator>();
builder.Services.AddScoped<IValidator<Tag>, TagValidator>();

// DATABASE
builder.Services.AddDbContext<DmsDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHostedService<OcrServiceSubscriber>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("Starting REST API");

app.Run();