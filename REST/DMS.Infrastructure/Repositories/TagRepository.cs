using DMS.Application;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories;

public class TagRepository(DmsDbContext dbContext, IEventDispatcher eventDispatcher, IValidator<Tag> validator)
    : BaseRepository<Tag>(dbContext, eventDispatcher, validator), ITagRepository
{
    public async Task<Tag?> GetByValue(string value)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Value == value);
    }
}