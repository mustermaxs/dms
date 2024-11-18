namespace DMS.Application.Interfaces
{
    // Elasticsearch
    public interface ISearchService
    {
        Task<List<SearchResult>> SearchAsync(string query);
        Task<bool> DeleteAsync(Guid id);
    }

    public class SearchResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string[] Tags { get; set; } = Array.Empty<string>();
    }
}