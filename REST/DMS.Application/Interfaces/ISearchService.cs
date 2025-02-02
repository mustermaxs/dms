using DMS.Domain.Entities;

namespace DMS.Application.Interfaces
{
    public interface ISearchService
    {
        Task<List<SearchResult>> SearchAsync(string query);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateDocumentAsync(DmsDocument document);
        public Task<bool> DeleteAllAsync();
    }

    public class SearchResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string[] Tags { get; set; } = Array.Empty<string>();
    }
}