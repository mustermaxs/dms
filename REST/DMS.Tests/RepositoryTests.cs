using AutoMapper;
using DMS.Domain.Entities.DmsDocument;
using DMS.Domain.Entities.Tags;
using DMS.Infrastructure;
using DMS.Infrastructure.Models;
using ProcessingStatus = DMS.Domain.Entities.DmsDocument.ValueObjects.ProcessingStatus;

namespace DMS.Tests.RepositoryTests;
using DMS.Domain.Entities;
using DMS.Domain.ValueObjects;

public class RepositoryTests
{

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateDocument_CreatesDocumentFromDomainEntityAndReturnsId()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<InfrastructureMappingProfile>());
        var mapper = config.CreateMapper();    
        var domainEntity = DmsDocument.Create(
            title: "Test Document.pdf",
            uploadDateTime: DateTime.Now,
            path: "Test Document.pdf",
            tags: [new Tag("Test Tag", "Test Value", "Test Color")],
            documentType: FileType.GetFileTypeFromExtension(".pdf"),
            status: ProcessingStatus.NotStarted
        );
        
        var persistenceEntity = mapper.Map<DmsDocument, DocumentModel>(domainEntity);
        
        Assert.That(persistenceEntity.Id, Is.EqualTo(domainEntity.Id));
        Assert.That(persistenceEntity.Title, Is.EqualTo(domainEntity.Title));
        Assert.That(persistenceEntity.UploadDateTime, Is.EqualTo(domainEntity.UploadDateTime));
        Assert.That(persistenceEntity.ModificationDateTime, Is.EqualTo(domainEntity.ModificationDateTime));
        Assert.That(persistenceEntity.Tags.All(pt => domainEntity.Tags.Any(dt => dt.Value == pt.Tag.Value)), Is.True);
    }
}