using Elastic.Clients.Elasticsearch;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;

namespace VideoService.Infrastructure.Search;

public sealed class ElasticVideoSearchIndex(ElasticsearchClient client) : IVideoSearchIndex
{
    public Task IndexVideoAsync(VideoSearchDocument document, CancellationToken ct)
        => client.IndexAsync(document, i => i
            .Index(ElasticIndexInitializer.IndexName)
            .Id(document.VideoId.ToString()), ct);

    public Task DeleteVideoAsync(VideoId videoId, CancellationToken ct)
        => client.DeleteAsync<VideoSearchDocument>(videoId.Value.ToString(), d => d
            .Index(ElasticIndexInitializer.IndexName), ct);

    public async Task<VideoSearchResult> SearchAsync(
        string? query, IReadOnlyList<string>? tags, int page, int pageSize, CancellationToken ct)
    {
        var response = await client.SearchAsync<VideoSearchDocument>(s => s
        .Indices(ElasticIndexInitializer.IndexName)
        .From((page - 1) * pageSize)
        .Size(pageSize)
        .Query(q =>
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                q.MultiMatch(mm => mm
                    .Query(query)
                    .Fields(new[] { "title^3", "description", "tags^2" })
                    .Fuzziness(new Fuzziness("AUTO")));
            }
            else
            {
                q.MatchAll(_ => { });
            }
        }), ct);

        var hits = response.Hits.Select(h => new VideoSearchHit(
            h.Source!.VideoId, h.Source.Title, h.Source.Description, h.Source.Tags,
            h.Source.ThumbnailUrl, h.Source.ViewsCount, h.Score ?? 0)).ToList();

        return new VideoSearchResult(hits, response.Total);
    }
}