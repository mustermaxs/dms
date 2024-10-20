using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using FluentValidation;

namespace DMS.Infrastructure.Repositories;

public class TagRepository(DmsDbContext dbContext, IEventDispatcher eventDispatcher, IValidator<Tag> validator)
    : BaseRepository<Tag>(dbContext, eventDispatcher, validator), ITagRepository;