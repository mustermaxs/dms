using DMS.Domain.Entities;
using DMS.Domain.Entities.Tags;

namespace DMS.Domain.IRepositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByValue(string value);
}