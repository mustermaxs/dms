using DMS.Application.DTOs;
using DMS.Application.Services;
using DMS.Domain.Entities.Tag;
using DMS.Domain.Services;
using DMS.Infrastructure.Services;
using DMS.Tests.DomainEvents.Mocks;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Minio.DataModel.Args;

namespace DMS.Tests.DmsDocuments;

public class DocumentUploadTests
{
    private IContainer? minioContainer;
    protected Givens Givens { get; private set; }

    [SetUp]
    public void Setup()
    {
        Givens = new Givens();
    }

    [TearDown]
    public async Task TearDown()
    {
        if (minioContainer != null)
        {
            await minioContainer.StopAsync();
            await minioContainer.DisposeAsync();
        }
    }

    [Test]
    public async Task FileStorage_Uploads_File()
    {
        // GIVEN
        minioContainer = await Givens.GivenMinioContainer();
        var minioClient = Givens.ServiceProvider.GetService<IMinioClient>();
        var fileStorage = Givens.GivenFileStorage(minioClient);
        var documentContenBase64 = Givens.GetMockBase64PdfContent();
        var fileHelper = Givens.ServiceProvider.GetService<FileHelper>();
        var fileContentStream = fileHelper.FromBase64ToStream(documentContenBase64);
        var minioConfig = Givens.GetMinioConfig();
        var bucketItems = new List<string>();
            
        // WHEN
        var fileName = await fileStorage.SaveFileAsync(new Guid(), fileContentStream);
        
        // THEN
        var listArgs = new ListObjectsArgs()
            .WithBucket(minioConfig.BucketName)
            .WithRecursive(true);
        
        await foreach (var item in minioClient.ListObjectsEnumAsync(listArgs).ConfigureAwait(false))
        {
            bucketItems.Add((item.Key));
        }
        Assert.That(bucketItems.Count, Is.EqualTo(1));
        Assert.That(bucketItems.Contains(fileName), Is.True);
    }
}