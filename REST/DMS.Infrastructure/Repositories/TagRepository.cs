using AutoMapper;
using DMS.Application;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.Entities.Tags;
using DMS.Domain.IRepositories;
using DMS.Infrastructure.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories;

public class TagRepository(DmsDbContext dbContext, IValidator<Tag> validator, IMapper mapper)
    : BaseRepository<Tag, TagModel>(dbContext, validator, mapper), ITagRepository
{
    public async Task<Tag?> GetByValue(string value)
    {
        var tagPersistence = await DbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Value == value);
        return mapper.Map<Tag>(tagPersistence);
    }

    public async Task<Guid> CreateIfNotExists(Tag entity)
    {
        var tagModel = mapper.Map<TagModel>(entity);
        var existingTag = await GetByValue(entity.Value);
        
        if (existingTag is not null)
            return existingTag.Id;
        var e = await DbSet.AddAsync(tagModel);
        await Context.SaveChangesAsync();
        return e.Entity.Id;
    }
}