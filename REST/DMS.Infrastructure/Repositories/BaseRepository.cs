using AutoMapper;
using DMS.Application;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using DMS.Infrastructure.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
public abstract class BaseRepository<TDomainEntity, TInfrastructureEntity> : IDisposable, IRepository<TDomainEntity>
where TDomainEntity : Entity
where TInfrastructureEntity : BasePersistenceModel
{
    public IValidator<TDomainEntity> Validator { get; }
    protected DmsDbContext Context;
    private readonly IMapper _mapper;
    private bool _disposed = false;
    protected DbSet<TInfrastructureEntity> DbSet;
    protected readonly IEventDispatcher _eventDispatcher;

    public BaseRepository(DmsDbContext dbContext, IValidator<TDomainEntity> validator, IMapper mapper)
    {
        Validator = validator;
        Context = dbContext;
        _mapper = mapper;
        DbSet = Context.Set<TInfrastructureEntity>();
    }
    
    public virtual async Task<TDomainEntity?> Get(Guid id)
    {
        var entity = await DbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return entity is not null ? _mapper.Map<TInfrastructureEntity, TDomainEntity>(entity) : null;
    }

    public virtual async Task<IEnumerable<TDomainEntity>?> GetAll()
    {
        var entities = await DbSet.AsNoTracking().ToListAsync();
        return entities.Select(e => _mapper.Map<TInfrastructureEntity, TDomainEntity>(e));
    }

    public virtual async Task<Guid> Create(TDomainEntity entity)
    {
        var persistenceEntity = _mapper.Map<TDomainEntity, TInfrastructureEntity>(entity);
        await Validator.ValidateAndThrowAsync(entity);
        var e = await DbSet.AddAsync(persistenceEntity);
        return e.Entity.Id;
    }

    public virtual async Task Delete(TDomainEntity entity)
    {
        var persistenceEntity = _mapper.Map<TDomainEntity, TInfrastructureEntity>(entity);
        DbSet.Remove(persistenceEntity);
    }
    
    public virtual async Task DeleteAllAsync()
    {
        DbSet.RemoveRange(DbSet);
    }

    public async Task SaveAsync()
    {
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteById(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null)
        {
            return;
        }
        DbSet.Remove(entity);
    }

    public virtual async Task UpdateAsync(TDomainEntity entity)
    {
        await Validator.ValidateAndThrowAsync(entity);
        var persistenceEntity = _mapper.Map<TDomainEntity, TInfrastructureEntity>(entity);
        DbSet.Update(persistenceEntity);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        try
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this._disposed = true;
        }
        catch (Exception e)
        {
            throw;
        }
    }

}}
