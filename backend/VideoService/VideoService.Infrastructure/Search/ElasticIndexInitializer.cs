using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;

namespace VideoService.Infrastructure.Search;

public static class ElasticIndexInitializer
{
    public const string IndexName = "videos";

    public static async Task EnsureIndexAsync(ElasticsearchClient client, CancellationToken ct = default)
    {
        var exists = await client.Indices.ExistsAsync(IndexName, ct);
        if (exists.Exists) return;

        await client.Indices.CreateAsync(IndexName, c => c
            .Settings(s => s
                .Analysis(a => a
                    .Tokenizers(t => t.EdgeNGram("edge_ngram_tokenizer", e => e
                        .MinGram(2).MaxGram(15)))
                    .Analyzers(an => an.Custom("autocomplete_analyzer", ca => ca
                        .Tokenizer("edge_ngram_tokenizer")
                        .Filter(["lowercase"])))))
            .Mappings(m => m.Properties(new Properties
            {
                { "title", new TextProperty { Analyzer = "autocomplete_analyzer", SearchAnalyzer = "standard", Boost = 3 } },
                { "description", new TextProperty { Analyzer = "standard" } },
                { "tags", new KeywordProperty() },
                { "videoId", new KeywordProperty() },
                { "authorId", new KeywordProperty() },
                { "publishedAtUtc", new DateProperty() },
                { "viewsCount", new LongNumberProperty() }
            })), ct);
    }
}