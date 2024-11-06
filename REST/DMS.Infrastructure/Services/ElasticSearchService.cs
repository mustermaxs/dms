using DMS.Application.Interfaces;

namespace DMS.Infrastructure.Services;

public class ElasticSearchService : ISearchService
{
    public Task IndexDocumentAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDocumentAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAllDocumentsAsync()
    {
        throw new NotImplementedException();
    }
}