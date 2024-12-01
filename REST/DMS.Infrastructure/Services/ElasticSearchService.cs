using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DMS.Infrastructure.Services;

public class ElasticSearchService : ISearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticSearchService> _logger;
    private const string IndexName = "documents";

    public ElasticSearchService(IConfiguration configuration, ILogger<ElasticSearchService> logger)
    {
        _logger = logger;
        var elasticUrl = configuration["ElasticSearch:Uri"] ?? throw new Exception($"Config for ElasticSearch not found");
        _logger.LogInformation($"Elasticsearch client initialized with URL: {elasticUrl}");
        _client = new ElasticsearchClient(new Uri(elasticUrl));
    }

    public async Task<List<SearchResult>> SearchAsync(string query)
    {
        try
        {
            var searchResponse = await _client.SearchAsync<SearchableDocument>(s => s
                .Index(IndexName)
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            sh => sh.QueryString(qs => qs.DefaultField(p => p.Content).Query($"*{query}*")),
                            sh => sh.QueryString(qs => qs.DefaultField(p => p.Title).Query($"*{query}*")),
                            sh => sh.QueryString(qs => qs.DefaultField(p => p.Tags).Query($"*{query}*"))
                        )
                    )
                )
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