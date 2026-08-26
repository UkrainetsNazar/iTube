using VideoService.Domain.Entities;

namespace VideoService.Application.Interfaces;

public interface IRecommendationFeedRepository
{
    Task<IReadOnlyList<Video>> GetTopViewedAsync(int count, CancellationToken ct);
    Task<IReadOnlyList<Video>> GetFeedAsync(int page, int pageSize, CancellationToken ct);
}