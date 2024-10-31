using AutoMapper;
using DMS.Application.Commands;
using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Application.Services;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;
using DMS.Infrastructure.Services;
using DMS.Tests.DomainEvents.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Minio.DataModel.Args;

namespace DMS.Tests.DmsDocuments;

public class DmsDocumentTests
{
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
    public async Task DocumentTagFactory_CreatesOnlyNewTags()
    {
        // GIVEN
        var novelTagDtos = new List<TagDto>
        {
            new TagDto
            {
                Id = Guid.Empty,
                Color = "#F0000",
                Label = "work",
                Value = "work"
            }
        };
        var tagFactory = Givens.ServiceProvider.GetService<IDocumentTagFactory>();
        var context = Givens.GetContext();
        
        // WHEN
        var newTags = await tagFactory!.CreateOrGetTagsFromTagDtos(novelTagDtos);
        await context.SaveChangesAsync();
        
        // THEN
        var tagsInDb = context.Tags.ToList();
        Assert.That(tagsInDb.Count, Is.EqualTo(1));
        Assert.That(tagsInDb[0].Label, Is.EqualTo("work"));
        Assert.That(tagsInDb[0].Value, Is.EqualTo("work"));
        Assert.That(tagsInDb[0].Color, Is.EqualTo("#F0000"));
        Assert.That(Guid.Empty, Is.Not.EqualTo(tagsInDb[0].Id));
    }

    [Test]
    public async Task FileStorage_DeleteFileAsync_DeletesFile()
    {
        // GIVEN
        var minioConfig = Givens.GetMinioConfig();
        var minioContainer = await Givens.GivenMinioContainer();
        var minioClient = Givens.ServiceProvider.GetService<IMinioClient>();
        var fileStorage = Givens.GivenFileStorage(minioClient);
        var fileHelper = Givens.ServiceProvider.GetService<FileHelper>();
        var fileId = Guid.NewGuid();
        var filePath = await fileStorage.SaveFileAsync(
            fileId,
            fileHelper.FromBase64ToStream(Givens.GivenPdfContentInBase64()));
        var bucketItems = new List<string>();

        // WHEN
        var result = await fileStorage.DeleteFileAsync(fileId);

        // THEN
        Assert.That(result, Is.True);
        
        var listArgs = new ListObjectsArgs()
            .WithBucket(minioConfig.BucketName)
            .WithRecursive(true);

        await foreach (var item in minioClient.ListObjectsEnumAsync(listArgs).ConfigureAwait(false))
        {
            bucketItems.Add((item.Key));
        }

        Assert.That(bucketItems.Count, Is.EqualTo(0));
    }
    //
    // [Test]
    // public async Task DeleteDocumentCommandHandler_DeletesDocument()
    // {
    //     // GIVEN
    //     var minioConfig = Givens.GetMinioConfig();
    //     var minioClient = Givens.GivenMinioClient();
    //     var fileStorage = Givens.GivenFileStorage(minioClient);
    //     var bucketItems = new List<string>();
    //     var document = await Givens.GivenDocumentInDbAndFileStorage();
    //     var deleteDocumentCommand = new DeleteDocumentCommand(document.Id);
    //     var deleteCommandHandler = new DeleteDocumentCommandHandler(
    //         fileStorage,
    //         Givens.ServiceProvider.GetRequiredService<IUnitOfWork>());
    //     
    //     var documentRepository = Givens.ServiceProvider.GetRequiredService<IDmsDocumentRepository>();
    //     
    //     // WHEN
    //     await deleteCommandHandler.Handle(deleteDocumentCommand, CancellationToken.None);
    //     
    //     // THEN
    //     var dbResult = await documentRepository.Get(document.Id);
    //     var listArgs = new ListObjectsArgs()
    //         .WithBucket(minioConfig.BucketName)
    //         .WithRecursive(true);
    //     
    //     await foreach (var item in minioClient.ListObjectsEnumAsync(listArgs).ConfigureAwait(false))
    //     {
    //         bucketItems.Add((item.Key));
    //     }
    //     Assert.That(bucketItems.Count, Is.EqualTo(0));
    //     Assert.That(bucketItems.Contains(document.Id.ToString()), Is.False);
    //     Assert.That(dbResult, Is.Null);
    // }
}