using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;

namespace DMS.Infrastructure.Repositories;

public class DocumentTagRepository(DmsDbContext dbContext, IDomainEventDispatcher eventDispatcher)
    : BaseRepository<DocumentTag>(dbContext, eventDispatcher),
        IDocumentTagRepository
{
    public async Task<DocumentTag> CreateOrGetIfExists(DocumentTag documentTag)
    {
        var existingTag = await Get(documentTag.TagId);
        return existingTag ?? await Create(documentTag);
    }
}