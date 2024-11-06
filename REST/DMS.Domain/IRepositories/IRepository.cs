using DMS.Domain.Entities;

namespace DMS.Domain.IRepositories;

public interface IRepository<TEntity> : IDisposable where TEntity : Entity
{
    Task<TEntity?> Get(Guid id);
    Task<IEnumerable<TEntity>?> GetAll();
    Task<TEntity> Create(TEntity entity);
    Task Delete(TEntity entity);
    Task DeleteById(Guid id);
    Task UpdateAsync(TEntity entity);
    Task SaveAsync();
    Task DeleteAllAsync();
}