using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories;

public class DmsDocumentRepository(DmsDbContext dbContext, IDomainEventDispatcher eventDispatcher)
    : BaseRepository<DmsDocument>(dbContext, eventDispatcher), IDmsDocumentRepository
{
    public async Task<DmsDocument> GetDocumentByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<DmsDocument?> Get(Guid id)
    {
        return await DbSet
            .Include(e => e.Tags)
            .ThenInclude(e => e.Tag)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public override async Task<IEnumerable<DmsDocument>?> GetAll()
    {
        return await DbSet
            .Include(e => e.Tags)
            .ThenInclude(e => e.Tag)
            .ToListAsync();
    }


    public override async Task<DmsDocument> Create(DmsDocument entity)
    {
        
    }
}