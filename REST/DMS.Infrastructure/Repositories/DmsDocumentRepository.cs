using AutoMapper;
using DMS.Application;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Documents;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;
using DMS.Infrastructure.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories;

public class DmsDocumentRepository(
    DmsDbContext dbContext,
    IValidator<DmsDocument> validator,
    IMapper mapper,
    IDocumentTagFactory documentTagFactory)
    : BaseRepository<DmsDocument, DocumentModel>(dbContext, validator, mapper), IDmsDocumentRepository
{
    public async Task<DmsDocument?> GetDocumentByIdAsync(Guid id)
    {
        var model =  await DbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return Mapper.Map<DmsDocument>(model);
    }

    public override async Task<DmsDocument> Create(DmsDocument domainEntity)
    {
        var tags = await documentTagFactory.CreateNewTagsOrGetExisting(domainEntity.Tags);
        var model = Mapper.Map<DocumentModel>(domainEntity);
        List<DocumentTagModel> tagModels = new List<DocumentTagModel>();
        tags.ForEach(tag => tagModels.Add(
            new DocumentTagModel { TagId = tag.Id, DocumentId = domainEntity.Id , Id = Guid.NewGuid(), TagModels = tag, DocumentModels = model}));
        model.Tags = tagModels;
        var entityEntry = await DbSet.AddAsync(model);
        return Mapper.Map<DmsDocument>(entityEntry.Entity);
    }

    public async Task<DmsDocument?> Get(Guid id)
    {
        var model = await DbSet.AsNoTracking()
            .Include(e => e.Tags)
            .ThenInclude(e => e.TagModels)
            .FirstOrDefaultAsync(e => e.Id == id);
        
        return model is not null ?  Mapper.Map<DmsDocument>(model) : null;
    }

    public override async Task<IEnumerable<DmsDocument>?> GetAll()
    {
        var models = await DbSet.AsNoTracking()
            .Include(e => e.Tags)
            .ThenInclude(e => e.TagModels)
            .ToListAsync();
        
        return Mapper.Map<IEnumerable<DmsDocument>>(models);
    }

    public override async Task UpdateAsync(DmsDocument entity)
    {
        var updatedTagModelList = await documentTagFactory.CreateNewTagsOrGetExisting(entity.Tags);
        var model = Mapper.Map<DocumentModel>(entity);
        List<DocumentTagModel> tagModels = new List<DocumentTagModel>();
        updatedTagModelList.ForEach(tag => tagModels.Add(
            new DocumentTagModel { TagId = tag.Id, DocumentId = model.Id , Id = Guid.NewGuid(), TagModels = tag, DocumentModels = model}));
        model.Tags = tagModels;
        DbSet.Entry(model).State = EntityState.Modified;
        DbSet.Update(model);
    }
}