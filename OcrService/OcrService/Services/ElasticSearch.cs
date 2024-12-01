using Microsoft.Extensions.Configuration;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using OcrService.Configs;

namespace OcrService;

public class ElasticSearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticSearchService> _logger;
    private const string IndexName = "documents";

    public ElasticSearchService(ElasticSearchConfig config, ILogger<ElasticSearchService> logger = null)
    {
        var uri = new Uri(config.Uri);
        _client = new ElasticsearchClient(uri);
        _logger = logger;
    }

    public async Task IndexDocumentAsync(Guid id, string content, string title, List<string> tags)
    {
        try
        {
            var exists = await _client.Indices.ExistsAsync(IndexName);
            if (!exists.Exists)
            {
                await _client.Indices.CreateAsync(IndexName);
            }

            var document = new SearchableDocument
            {
                Id = id,
                Content = content,
                Title = title,
                Tags = tags?.ToArray() ?? Array.Empty<string>()
            };

            var response = await _client.IndexAsync(document, IndexName, id.ToString());
            
            if (!response.IsValidResponse)
            {
                _logger?.LogError("Failed to index document: {Error}", response.DebugInformation);
                throw new Exception($"Failed to index document: {response.DebugInformation}");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error indexing document {Id}", id);
            throw;
        }
    }
    
}

public class SearchableDocument
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public string[] Tags { get; set; }
}