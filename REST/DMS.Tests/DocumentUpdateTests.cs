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

public class DocumentUpdateTests
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
    public async Task UpdateDocument_IgnoresPreexistingTags()
    {
        // GIVEN
        var repo = Givens.ServiceProvider.GetRequiredService<IDmsDocumentRepository>();
        var tag1 = new Tag("test", "test", "#F0000");
        var tag2 = new Tag("test", "test", "#F0000");
        var document = DmsDocument.Create("testDocument.pdf",
            DateTime.UtcNow,
            "",
            new List<DocumentTag>
            {
            },
            new FileType("testDocument.pdf"),
            ProcessingStatus.NotStarted);
        document.AddTag(tag1);
        await repo.Create(document);
        await repo.SaveAsync();
        
        // WHEN
        var preexistingDocTags = (await repo.Get((document.Id))!).Tags;
        var preexistingTag = preexistingDocTags.ToList().First();      
        document.UpdateTags([new Tag(preexistingTag.Tag.Label, preexistingTag.Tag.Value, preexistingTag.Tag.Color)]);
        await repo.UpdateAsync(document);
        await repo.SaveAsync();
        
        // THEN
        var documentInDb = await repo.Get(document.Id);
        Assert.That(documentInDb.Tags.Count, Is.EqualTo(1));
    }
}