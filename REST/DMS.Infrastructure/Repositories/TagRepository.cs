using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;

namespace DMS.Infrastructure.Repositories;

public class TagRepository(DmsDbContext dbContext, IEventDispatcher eventDispatcher)
    : BaseRepository<Tag>(dbContext, eventDispatcher), ITagRepository;