using Shared.Domain.ValueObjects;
using VideoService.Domain.Entities;

namespace VideoService.Application.Interfaces;

public interface IRecommendationFeedRepository
{
    Task<IReadOnlyList<Video>> GetFeedAsync(int page, int pageSize, CancellationToken ct);
    Task<IReadOnlyList<Video>> GetByTagOverlapAsync(IReadOnlyList<string> tags, VideoId? excludeVideoId, int limit, CancellationToken ct);
    Task<IReadOnlyList<Video>> GetByUserPreferenceAsync(Guid userId, int limit, CancellationToken ct);
    Task<IReadOnlyList<Video>> GetTopViewedAsync(int limit, IReadOnlyCollection<VideoId> excludeIds, CancellationToken ct);
}