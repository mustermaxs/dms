using DMS.Domain.Entities;

namespace DMS.Domain.IRepositories;

public interface IDocumentTagRepository : IRepository<DocumentTag>
{
    Task<DocumentTag> CreateOrGetIfExists(DocumentTag documentTag);
    Task DeleteAllByDocumentId(Guid documentId);
    public Task<List<DocumentTag>> GetAllByDocumentId(Guid tagId);

}