using AutoMapper;
using DMS.Domain.Entities.Tags;
using DMS.Domain.IRepositories;
using DMS.Infrastructure.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories;

public class TagRepository(DmsDbContext dbContext, IValidator<Tag> validator, IMapper mapper)
    : BaseRepository<Tag, TagModel>(dbContext, validator, mapper), ITagRepository
{
    public async Task<Tag?> GetByValue(string value)
    {
        var model = await DbSet.FirstOrDefaultAsync(e => e.Value == value);
        return Mapper.Map<Tag>(model);
    }

    public override async Task<IEnumerable<Tag>> GetAll()
    {
        var tagModels = await DbSet.ToListAsync();
        return Mapper.Map<IEnumerable<Tag>>(tagModels);
    }
}