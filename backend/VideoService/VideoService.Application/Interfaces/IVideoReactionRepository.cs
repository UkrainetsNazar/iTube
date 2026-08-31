using Shared.Domain.ValueObjects;
using VideoService.Domain.Entities;

namespace VideoService.Application.Interfaces;

public interface IVideoReactionRepository
{
    Task<VideoReaction?> GetByVideoAndUserAsync(VideoId videoId, Guid userId, CancellationToken ct);
    Task<(IReadOnlyList<VideoId> VideoIds, int TotalCount)> GetLikedVideoIdsByUserAsync(Guid userId, int page, int pageSize, CancellationToken ct);
    void Add(VideoReaction reaction);
    void Update(VideoReaction reaction);
    void Remove(VideoReaction reaction);
}