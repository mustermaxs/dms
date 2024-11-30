using AutoMapper;
using DMS.Application;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
public abstract class BaseRepository<TDomainEntity, TModelEntity> : IDisposable, IRepository<TDomainEntity>
where TDomainEntity : Entity
where TModelEntity : class
{
    public IValidator<TDomainEntity> Validator { get; }
    public IMapper Mapper { get; }
    protected DmsDbContext Context;
    private bool _disposed = false;
    protected DbSet<TModelEntity> DbSet;
    protected readonly IEventDispatcher _eventDispatcher;

    public BaseRepository(DmsDbContext dbContext, IValidator<TDomainEntity> validator, IMapper mapper)
    {
        Validator = validator;
        Mapper = mapper;
        Context = dbContext;
        DbSet = Context.Set<TModelEntity>();
    }
    
    public virtual async Task<TDomainEntity?> Get(Guid id)
    {
        var model = await DbSet.FindAsync(id);
        var domainEntity = Mapper.Map<TDomainEntity>(model);
        return domainEntity;
    }

    public virtual async Task<IEnumerable<TDomainEntity>?> GetAll()
    {
        var models = await DbSet.ToListAsync();
        return Mapper.Map<IEnumerable<TDomainEntity>>(models);
    }

    public virtual async Task<TDomainEntity> Create(TDomainEntity domainEntity)
    {
        var model = Mapper.Map<TModelEntity>(domainEntity);
        var entityEntry = await DbSet.AddAsync(model);
        var createdModel = Mapper.Map<TDomainEntity>(entityEntry.Entity);
        return createdModel;
    }

    public virtual async Task Delete(TDomainEntity entity)
    {
        var model = Mapper.Map<TModelEntity>(entity);
        DbSet.Remove(model);
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
        var model = await DbSet.FindAsync(id);
        if (model == null)
        {
            return;
        }
        DbSet.Remove(model);
        
    }

    public virtual async Task UpdateAsync(TDomainEntity entity)
    {
        var model = Mapper.Map<TModelEntity>(entity);
        DbSet.Update(model);
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
