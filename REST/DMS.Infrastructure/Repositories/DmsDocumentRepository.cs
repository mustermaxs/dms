using AutoMapper;
using DMS.Application.Interfaces;
using DMS.Domain.Entities.DmsDocument;
using DMS.Domain.IRepositories;
using DMS.Infrastructure.Models;
using DMS.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories;

public class DmsDocumentRepository(DmsDbContext dbContext, IValidator<DmsDocument> validator, IMapper mapper, ITagCreateService documentTagService)
    : BaseRepository<DmsDocument, DocumentModel>(dbContext, validator, mapper), IDmsDocumentRepository
{
    public override async Task<Guid> Create(DmsDocument entity)
    {
        var tags = await documentTagService.CreateOrGetTagsFromTagDtos(entity.Tags);
        entity.SetTags(tags);
        await validator.ValidateAndThrowAsync(entity);
        var persistenceEntity = mapper.Map<DmsDocument, DocumentModel>(entity);
        var e = await DbSet.AddAsync(persistenceEntity);
        return e.Entity.Id;
    }

    public override async Task<IEnumerable<DmsDocument>?> GetAll()
    {
        var documentsPersistence = await DbSet.AsNoTracking()
            .Include(d => d.Tags)
            .ThenInclude(t => t.Tag)
            .ToListAsync();

        return mapper.Map<IEnumerable<DmsDocument>>(documentsPersistence);
    }

    public override async Task UpdateAsync(DmsDocument entity)
    {
        // TODO use tag service to get new tags to update
        await Validator.ValidateAndThrowAsync(entity);
        var persistenceEntity = await DbSet.FindAsync(entity.Id);
        if (persistenceEntity == null)
            throw new ArgumentException($"Entity with id {entity.Id} not found");
        var updatedPersistenceEntity = mapper.Map(entity, persistenceEntity);
        DbSet.Entry(updatedPersistenceEntity).State = EntityState.Modified;
        DbSet.Update(updatedPersistenceEntity);
    }
}