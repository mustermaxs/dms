using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;

namespace DMS.Infrastructure.Repositories;

public class DmsDocumentRepository(DmsDbContext dbContext, IDomainEventDispatcher eventDispatcher)
    : BaseRepository<DmsDocument>(dbContext, eventDispatcher), IDmsDocumentRepository
{
    public async Task<DmsDocument> GetDocumentByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }
}