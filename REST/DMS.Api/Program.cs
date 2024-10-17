using System.Reflection;
using DMS.Application.Commands;
using DMS.Application.Interfaces;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.Entities.DomainEvents;
using DMS.Domain.IRepositories;
using DMS.Infrastructure;
using DMS.Infrastructure.EventHandlers;
using DMS.Infrastructure.Repositories;
using DMS.Infrastructure.Services;
using FluentValidation;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
// using DMS.Domain.Repositories;
// using DMS.Infrastructure.Repositories;
// using DMS.Infrastructure.Persistence;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// COMMANDS & QUERIES
builder.Services.AddMediatR(
    typeof(UploadDocumentCommand).Assembly
    );

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

// LOGGING
builder.Logging.ClearProviders(); 
builder.Logging.AddLog4Net();  

// SERVICES
builder.Services.AddScoped<IDocumentTagService, DocumentTagService>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IDomainEventHandler<DocumentSavedInDbEvent>, DocumentSavedInDbEventHandler>();
builder.Services.AddScoped<IMessageBrokerClient, RabbitMqClient>();
builder.Services.AddScoped<IFileStorage, FileStorage>();

// REPOSITORIES
builder.Services.AddScoped<IDmsDocumentRepository, DmsDocumentRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// VALIDATORS
builder.Services.AddScoped<IValidator<DmsDocument>, DmsDocumentValidator>();

builder.Services.AddDbContext<DmsDbContext>(options =>
    options.UseNpgsql(connectionString));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("Starting REST API");

app.Run();