using System.Reflection;
using DMS.Application.Commands;
using DMS.Domain;
using DMS.Domain.Entities.DomainEvents;
using DMS.Domain.IRepositories;
using DMS.Infrastructure;
using DMS.Infrastructure.EventHandlers;
using DMS.Infrastructure.Repositories;
using DMS.Infrastructure.Services;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
// using DMS.Domain.Repositories;
// using DMS.Infrastructure.Repositories;
// using DMS.Infrastructure.Persistence;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(
    typeof(UploadDocumentCommand).Assembly
    );

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// use log4net for logging
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

// Clear default logging providers and add log4net
builder.Logging.ClearProviders();  // Remove other logging providers (optional)
builder.Logging.AddLog4Net();  
// Add PostgreSQL support with Entity Framework Core
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<DMSDbContext>(options =>
//options.UseNpgsql(connectionString));

// Register repositories and services
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IDomainEventHandler<DocumentUploadedEvent>, DocumentCreatedEventHandler>();
builder.Services.AddScoped<IMessageBrokerClient, RabbitMqClient>();
builder.Services.AddScoped<IDmsDocumentRepository, DmsDocumentRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
// builder.Services.AddScoped<IProductRepository, ProductRepository>();
// builder.Services.AddScoped<IProductService, ProductService>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DmsDbContext>(options =>
    options.UseNpgsql(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
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