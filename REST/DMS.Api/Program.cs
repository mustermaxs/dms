using System.Reflection;
using AutoMapper;
using DMS.Application.Commands;
using DMS.Application.DTOs;
using DMS.Application.IntegrationEvents;
using DMS.Application.Interfaces;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.Entities.DomainEvents;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;
using DMS.Domain.ValueObjects;
using DMS.Infrastructure;
using DMS.Infrastructure.EventHandlers;
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
    typeof(DocumentSavedInFileStorageIntegrationEvent).Assembly
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
builder.Services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
builder.Services.AddScoped<IMessageBroker, RabbitMqClient>();
// builder.Services.AddScoped<IFileStorage, FileStorage>();
// builder.Services.AddScoped<IIntegrationEventHandler<DocumentSavedInFileStorageIntegrationEvent>, DocumentSavedInFileStorageEventHandler>();

// MINIO
// var minioConfig = builder.Configuration.GetSection("MinIO").Get<MinioConfig>();
// builder.Services.AddMinio(cgf => cgf
//     .WithEndpoint(minioConfig.Endpoint)
//     .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
//     .Build());

// REPOSITORIES
builder.Services.AddScoped<IDmsDocumentRepository, DmsDocumentRepository>();
builder.Services.AddScoped<IDocumentTagRepository, DocumentTagRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddScoped<IProductRepository, ProductRepository>();
// builder.Services.AddScoped<IProductService, ProductService>();

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