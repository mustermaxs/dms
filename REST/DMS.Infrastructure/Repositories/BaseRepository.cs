using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
public abstract class BaseRepository<TEntity> : IDisposable
where TEntity : class
{
    protected DmsDbContext Context;
    private bool _disposed = false;
    protected DbSet<TEntity> DbSet;

    public BaseRepository(DmsDbContext dbContext)
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
    }

    public virtual async Task Delete(TEntity entity)
    {
        DbSet.Remove(entity);
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
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
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
