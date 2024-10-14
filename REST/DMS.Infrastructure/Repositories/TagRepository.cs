using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;

namespace DMS.Infrastructure.Repositories;

public class TagRepository(DmsDbContext dbContext, IDomainEventDispatcher eventDispatcher)
    : BaseRepository<Tag>(dbContext, eventDispatcher), ITagRepository;