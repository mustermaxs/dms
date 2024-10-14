using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
public abstract class BaseRepository<TEntity> : IDisposable, IRepository<TEntity>
where TEntity : Entity
{
    protected DmsDbContext Context;
    private bool _disposed = false;
    protected DbSet<TEntity> DbSet;
    protected readonly IDomainEventDispatcher _eventDispatcher;

    public BaseRepository(DmsDbContext dbContext, IDomainEventDispatcher _eventDispatcher)
    {
        Context = dbContext;
        DbSet = Context.Set<TEntity>();
    }
    
    public virtual async Task<TEntity?> Get(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>?> GetAll()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task Create(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await SaveAsync();
        await _eventDispatcher.DispatchEventsAsync(entity.DomainEvents.ToList());
    }

    public virtual async Task Delete(TEntity entity)
    {
        DbSet.Remove(entity);
        await SaveAsync();
        await _eventDispatcher.DispatchEventsAsync(entity.DomainEvents.ToList());
    }

    public async Task SaveAsync()
    {
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteById(int id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null)
        {
            return;
        }
        DbSet.Remove(entity);
        await SaveAsync();
        await _eventDispatcher.DispatchEventsAsync(entity.DomainEvents.ToList());
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        await SaveAsync();
        await _eventDispatcher.DispatchEventsAsync(entity.DomainEvents.ToList());
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
