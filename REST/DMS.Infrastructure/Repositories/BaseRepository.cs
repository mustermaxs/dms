using DMS.Application;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
public abstract class BaseRepository<TEntity> : IDisposable, IRepository<TEntity>
where TEntity : Entity
{
    public IValidator<TEntity> Validator { get; }
    protected DmsDbContext Context;
    private bool _disposed = false;
    protected DbSet<TEntity> DbSet;
    protected readonly IEventDispatcher _eventDispatcher;

    public BaseRepository(DmsDbContext dbContext, IValidator<TEntity> validator)
    {
        Validator = validator;
        Context = dbContext;
        DbSet = Context.Set<TEntity>();
    }
    
    public virtual async Task<TEntity?> Get(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>?> GetAll()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<TEntity> Create(TEntity entity)
    {
        await Validator.ValidateAndThrowAsync(entity);
        var e = await DbSet.AddAsync(entity);
        
        return e.Entity;
    }

    public virtual async Task Delete(TEntity entity)
    {
        DbSet.Remove(entity);
        
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

    public virtual async Task UpdateAsync(TEntity entity)
    {
        await Validator.ValidateAndThrowAsync(entity);
        DbSet.Update(entity);
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
