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

    public BaseRepository(DmsDbContext dbContext, IDomainEventDispatcher eventDispatcher)
    {
        Context = dbContext;
        DbSet = Context.Set<TEntity>();
        _eventDispatcher = eventDispatcher;
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
        entity.Id = Guid.NewGuid();
        var e = await DbSet.AddAsync(entity);
        await SaveAsync();
        return e.Entity;
    }

    public virtual async Task Delete(TEntity entity)
    {
        DbSet.Remove(entity);
        await SaveAsync();
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
        await SaveAsync();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        await SaveAsync();
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
