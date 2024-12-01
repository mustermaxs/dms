using System.Linq.Expressions;
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

    // https://arjanvanbekkum.github.io/blog/2020/12/14/Update-Entity-Framework-Child-Records
    [Test]
    [Ignore("Unfixed error: Attempted to update or delete an entity that does not exist in the store.")]
    public async Task UpdateDocument_IgnoresPreexistingTags()
    {
        // GIVEN
        var repo = Givens.ServiceProvider.GetRequiredService<IDmsDocumentRepository>();
        var tag1 = Tag.Create("test", "test", "#F0000");
        var tag2 = Tag.Create("test", "test", "#F0000");
        var document = DmsDocument.Create("testDocument.pdf",
            DateTime.UtcNow,
            "",
            new List<DocumentTag>
            {
            },
            "testDocument.pdf",
            ProcessingStatus.NotStarted);
        document.AddTag(tag1);
        await repo.Create(document);
        await repo.SaveAsync();
        var documentFromDb = await repo.Get(document.Id);
        
        // WHEN
        var preexistingDocTags = documentFromDb!.Tags;
        var preexistingTag = preexistingDocTags!.ToList().First();
        var newTag = Tag.Create(preexistingTag.Tag.Label, preexistingTag.Tag.Value, preexistingTag.Tag.Color);
        documentFromDb.UpdateTags([newTag]);
        await repo.UpdateAsync(documentFromDb);
        await repo.SaveAsync();
        
        // THEN
        var documentInDb = await repo.Get(document.Id);
        Assert.That(documentInDb!.Tags.Count, Is.EqualTo(1));
    }
}