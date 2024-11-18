using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DMS.Infrastructure.Services;

public class ElasticSearchService : ISearchService
{
    private ElasticsearchClient _client;
    private readonly ILogger<ElasticSearchService> _logger;
    private const string IndexName = "documents";

    public ElasticSearchService(IConfiguration configuration, ILogger<ElasticSearchService> logger)
    {
        var uri = new Uri("http://localhost:9200");
        _client = new ElasticsearchClient(uri);
        _logger = logger;
    }

    public async Task<List<SearchResult>> SearchAsync(string query)
    {
        Console.WriteLine(query);
        try
        {
            var searchResponse = await _client.SearchAsync<SearchableDocument>(s => s
                .Index(IndexName)
                .Query(q => q.QueryString(qs => qs.DefaultField(p => p.Content).Query($"*{query}*")))
            );

            if (!searchResponse.IsValidResponse)
            {
                _logger.LogError("Failed to search documents: {Error}", searchResponse.DebugInformation);
                throw new Exception($"Failed to search documents: {searchResponse.DebugInformation}");
            }

            return searchResponse.Hits.Select(hit => new SearchResult
            {
                Id = hit.Source.Id,
                Title = hit.Source.Title,
                Tags = hit.Source.Tags,
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching documents");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleteResponse = await _client.DeleteAsync(new DeleteRequest(IndexName, id));

        return deleteResponse.IsValidResponse;
    }

}


public class SearchableDocument
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
}