using System.Reflection;
using DMS.Api;
using DMS.Application;
using DMS.Application.Commands;
using DMS.Application.DTOs;
using DMS.Application.IntegrationEvents;
using DMS.Application.Interfaces;
using DMS.Application.Services;
using DMS.Domain.Entities;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;
using DMS.Infrastructure;
using DMS.Infrastructure.Repositories;
using DMS.Infrastructure.Services;
using FluentValidation;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Minio;

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
    typeof(DocumentTagsUpdatedDomainEvent).Assembly
    );

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AUTOMAPPER
builder.Services.AddAutoMapper( typeof(DmsMappingProfile));

// LOGGING
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
builder.Logging.ClearProviders(); 
builder.Logging.AddLog4Net();  

// SERVICES
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDocumentTagFactory, DocumentTagFactory>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<IMessageBroker, RabbitMqClient>();
builder.Services.AddTransient<FileHelper>();

// MINIO
builder.Services.AddScoped<IFileStorage, FileStorage>( sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var minioConfig = config.GetSection("MinIO").Get<DmsMinioConfig>();
    return new FileStorage(minioConfig.AccessKey, minioConfig.SecretKey, minioConfig.Endpoint, minioConfig.BucketName);
});

// builder.Services.AddMinio(cgf => cgf
//     .WithEndpoint(minioConfig.Endpoint)
//     .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
//     .Build());

// REPOSITORIES
builder.Services.AddScoped<IDmsDocumentRepository, DmsDocumentRepository>();
builder.Services.AddScoped<IDocumentTagRepository, DocumentTagRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// VALIDATORS
builder.Services.AddScoped<IValidator<DmsDocument>, DmsDocumentValidator>();
builder.Services.AddScoped<IValidator<Tag>, TagValidator>();
builder.Services.AddScoped<IValidator<DocumentTag>, DocumentTagValidator>();

// CONFIGS

// DATABASE
builder.Services.AddDbContext<DmsDbContext>(options =>
    options.UseNpgsql(connectionString));

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