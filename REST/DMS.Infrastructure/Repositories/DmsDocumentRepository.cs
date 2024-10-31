using DMS.Application;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories;

public class DmsDocumentRepository(DmsDbContext dbContext, IEventDispatcher eventDispatcher, IValidator<DmsDocument> validator)
    : BaseRepository<DmsDocument>(dbContext, eventDispatcher, validator), IDmsDocumentRepository
{
    public async Task<DmsDocument> GetDocumentByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public override async Task<DmsDocument> Create(DmsDocument entity)
    {
        await validator.ValidateAndThrowAsync(entity);
        var e = await DbSet.AddAsync(entity);
        return e.Entity;
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

    public override async Task UpdateAsync(DmsDocument entity)
    {
        await Validator.ValidateAndThrowAsync(entity);
        DbSet
        .Update(entity);
    }
}