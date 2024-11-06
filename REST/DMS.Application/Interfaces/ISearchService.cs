namespace DMS.Application.Interfaces;

// Elasticsearch
public interface ISearchService
{
    Task IndexDocumentAsync(Guid id);
    Task DeleteDocumentAsync(Guid id);
    Task DeleteAllDocumentsAsync();
}