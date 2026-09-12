using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Microsoft.Extensions.Logging;

namespace VideoService.Infrastructure.Search;

public static class ElasticIndexInitializer
{
    public const string IndexName = "videos";

    public static async Task EnsureIndexAsync(ElasticsearchClient client, ILogger logger, CancellationToken ct = default)
    {
        var existsResponse = await client.Indices.ExistsAsync(IndexName, ct);

        if (!existsResponse.ApiCallDetails.HasSuccessfulStatusCode && existsResponse.ApiCallDetails.HttpStatusCode != 404)
            throw new InvalidOperationException(
                $"Failed to check whether Elasticsearch index '{IndexName}' exists. HTTP {existsResponse.ApiCallDetails.HttpStatusCode}. {existsResponse.DebugInformation}");

        if (existsResponse.Exists)
        {
            logger.LogInformation("Elasticsearch index '{IndexName}' already exists.", IndexName);
            return;
        }

        logger.LogInformation("Elasticsearch index '{IndexName}' not found — creating it.", IndexName);

        var createResponse = await client.Indices.CreateAsync(IndexName, c => c
            .Settings(s => s
                .Analysis(a => a
                    .TokenFilters(tf => tf.EdgeNGram("edge_ngram_filter", e => e
                        .MinGram(2).MaxGram(15)))
                    .Analyzers(an => an.Custom("autocomplete_analyzer", ca => ca
                        .Tokenizer("standard")
                        .Filter(["lowercase", "edge_ngram_filter"])))))
            .Mappings(m => m.Properties(new Properties
            {
                {
                    "title", new TextProperty
                    {
                        Analyzer = "standard",
                        Fields = new Properties
                        {
                            { "autocomplete", new TextProperty { Analyzer = "autocomplete_analyzer", SearchAnalyzer = "standard" } }
                        }
                    }
                },
                {
                    "description", new TextProperty
                    {
                        Analyzer = "standard",
                        Fields = new Properties
                        {
                            { "autocomplete", new TextProperty { Analyzer = "autocomplete_analyzer", SearchAnalyzer = "standard" } }
                        }
                    }
                },
                { "tags", new KeywordProperty() },
                { "videoId", new KeywordProperty() },
                { "authorId", new KeywordProperty() },
                { "publishedAtUtc", new DateProperty() },
                { "viewsCount", new LongNumberProperty() }
            })), ct);

        if (!createResponse.IsValidResponse)
            throw new InvalidOperationException($"Failed to create Elasticsearch index '{IndexName}'. {createResponse.DebugInformation}");

        logger.LogInformation("Elasticsearch index '{IndexName}' created successfully.", IndexName);
    }
}