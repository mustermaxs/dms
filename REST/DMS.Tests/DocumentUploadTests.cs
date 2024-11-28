using DMS.Application.DTOs;
using DMS.Application.Services;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;
using DMS.Domain.ValueObjects;
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
    public async Task Teardown()
    {
        await Givens.DisposeAsync();
    }

    [Test]
    public async Task FileStorage_Uploads_File()
    {
        // GIVEN
        minioContainer = await Givens.GivenMinioContainer();
        var minioClient = Givens.ServiceProvider.GetService<IMinioClient>();
        var fileStorage = Givens.GivenFileStorage(minioClient);
        var documentContenBase64 = Givens.GivenPdfContentInBase64();
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

    [Test]
    public async Task DmsDocumentRepository_Creates_New_Document()
    {
        // GIVEN
        var repo = Givens.ServiceProvider.GetRequiredService<IDmsDocumentRepository>();
        var document = DmsDocument.Create("testDocument.pdf",
            DateTime.UtcNow,
            "",
            new List<DocumentTag>(),
            "testDocument.pdf",
            ProcessingStatus.NotStarted);
        
        // WHEN
        await repo.Create(document);
        await repo.SaveAsync();
        
        // THEN
        var documentFromDb = await repo.Get(document.Id);
        Assert.That(documentFromDb, Is.Not.Null);
        Assert.That(documentFromDb.Id, Is.EqualTo(document.Id));
        Assert.That(documentFromDb.Title, Is.EqualTo(document.Title));
    }

    [Test]
    public async Task DmsDocumentRepository_Creates_New_Document_With_Tags()
    {
        // GIVEN & WHEN
        var repo = Givens.ServiceProvider.GetRequiredService<IDmsDocumentRepository>();
        var tag1 = new Tag("test", "test", "#F0000");
        var tag2 = new Tag("test", "test", "#F0000");
        var document = DmsDocument.Create("testDocument.pdf",
            DateTime.UtcNow,
            "",
            new List<DocumentTag>
            {
            },
            "testDocument.pdf",
            ProcessingStatus.NotStarted);
        document.AddTag(tag1);
        document.AddTag(tag2);
        await repo.Create(document);
        await repo.SaveAsync();

        // THEN
        var documentFromDb = await repo.Get(document.Id);
        Assert.That(documentFromDb, Is.Not.Null);
        Assert.That(documentFromDb.Id, Is.EqualTo(document.Id));
        Assert.That(documentFromDb.Title, Is.EqualTo(document.Title));
        Assert.That(documentFromDb.Tags.Count, Is.EqualTo(2));
        Assert.That(documentFromDb.Tags.ToList()[0].Tag.Label, Is.EqualTo(tag1.Label));
        Assert.That(documentFromDb.Tags.ToList()[1].Tag.Label, Is.EqualTo(tag2.Label));
    }
}