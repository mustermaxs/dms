using DMS.Application.DTOs;
using DMS.Domain.Entities.Tag;
using DMS.Domain.Services;
using DMS.Infrastructure.Services;
using DMS.Tests.DomainEvents.Mocks;
using Microsoft.Extensions.DependencyInjection;

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
   
}