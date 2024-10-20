using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using FluentValidation;

namespace DMS.Infrastructure.Repositories;

public class DocumentTagRepository(DmsDbContext dbContext, IEventDispatcher eventDispatcher, IValidator<DocumentTag> validator)
    : BaseRepository<DocumentTag>(dbContext, eventDispatcher, validator),
        IDocumentTagRepository
{
    public async Task<DocumentTag> CreateOrGetIfExists(DocumentTag documentTag)
    {
        // TODO also execute Tag validator here
        await validator.ValidateAndThrowAsync(documentTag);
        var existingTag = await Get(documentTag.TagId);
        return existingTag ?? await Create(documentTag);
    }
}