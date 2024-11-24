namespace DMS.Domain.IRepositories;
using DMS.Domain.Entities.Tags;
public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByValue(string value);
    public Task<Guid> CreateIfNotExists(Tag entity);
}