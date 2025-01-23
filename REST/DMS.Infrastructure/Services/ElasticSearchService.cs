using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;
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
        _client = new ElasticsearchClient(new Uri(elasticUrl));
        _logger.LogDebug($"Created ElasticSearchClient with url {elasticUrl}");
    }

    public async Task<List<SearchResult>> SearchAsync(string query)
    {
        try
        {
            _logger.LogInformation($"Searching for documents with query: {query}");
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
            _logger.LogError(ex, "Error searching documents. Query: {Query}", query);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var deleteResponse = await _client.DeleteAsync(new DeleteRequest(IndexName, id));
            
            if (!deleteResponse.IsValidResponse)
                _logger.LogError($"Failed to delete index for document with id: {id}");
            _logger.LogInformation($"Deleted index for document with id: {id}");
            return deleteResponse.IsValidResponse;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to delete index for document with id: {id}");
            throw;
        }
    }

    public async Task<bool> DeleteAllAsync()
    {
        try
        {
            var deleteResponse = await _client.DeleteAsync("documents");
            if (!deleteResponse.IsValidResponse)
                _logger.LogError($"Failed to delete index for documents");
            _logger.LogInformation($"Deleted index for documents");
            
            return deleteResponse.IsValidResponse;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to delete index for documents");
            throw;
        }
    }

    public async Task<bool> UpdateDocumentAsync(DmsDocument document)
    {
        var searchableDoc = new SearchableDocument
        {
            Id = document.Id,
            Content = document.Content,
            Title = document.Title,
            Tags = document.Tags.Select(t => t.Tag.Label).ToArray()
        };

        var indexResponse = await _client.IndexAsync(searchableDoc, 
            idx => idx.Index(IndexName).Id(document.Id));
        
        return indexResponse.IsValidResponse;
    }

}


public class SearchableDocument
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
}