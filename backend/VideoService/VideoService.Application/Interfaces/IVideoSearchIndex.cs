using Shared.Domain.ValueObjects;

namespace VideoService.Application.Interfaces;

public interface IVideoSearchIndex
{
    Task IndexVideoAsync(VideoSearchDocument document, CancellationToken ct);
    Task DeleteVideoAsync(VideoId videoId, CancellationToken ct);
    Task<VideoSearchResult> SearchAsync(string? query, IReadOnlyList<string>? tags, int page, int pageSize, CancellationToken ct);
}

public sealed record VideoSearchDocument(
    Guid VideoId, string Title, string Description, IReadOnlyList<string> Tags,
    Guid AuthorId, string? ThumbnailUrl, long ViewsCount, DateTime PublishedAtUtc);

public sealed record VideoSearchHit(
    Guid VideoId, string Title, string Description, IReadOnlyList<string> Tags,
    string? ThumbnailUrl, long ViewsCount, double Score);

public sealed record VideoSearchResult(IReadOnlyList<VideoSearchHit> Hits, long TotalCount);