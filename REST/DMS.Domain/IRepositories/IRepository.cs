using DMS.Domain.Entities;

namespace DMS.Domain.IRepositories;

public interface IRepository<TDomainEntity> : IDisposable 
    where TDomainEntity : Entity
{
    Task<TDomainEntity?> Get(Guid id);
    Task<IEnumerable<TDomainEntity>?> GetAll();
    Task<TDomainEntity> Create(TDomainEntity domainEntity);
    Task Delete(TDomainEntity entity);
    Task DeleteById(Guid id);
    Task UpdateAsync(TDomainEntity entity);
    Task SaveAsync();
    Task DeleteAllAsync();
}