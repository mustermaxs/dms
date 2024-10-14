using DMS.Domain.Entities;

namespace DMS.Domain.IRepositories;

public interface IRepository<TEntity> : IDisposable where TEntity : Entity
{
    Task<TEntity?> Get(int id);
    Task<IEnumerable<TEntity>?> GetAll();
    Task Create(TEntity entity);
    Task Delete(TEntity entity);
    Task DeleteById(int id);
    Task UpdateAsync(TEntity entity);
    Task SaveAsync();
}