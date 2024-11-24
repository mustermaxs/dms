namespace DMS.Domain.IRepositories;
using DMS.Domain.Entities.Tags;

public interface IDocumentTagRepository : IRepository<Tag>
{
    Task<Tag> CreateOrGetIfExists(Tag documentTag);
    Task DeleteAllByDocumentId(Guid documentId);
    public Task<List<Tag>> GetAllByDocumentId(Guid tagId);

}