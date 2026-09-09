using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Domain.Enums;
using VideoService.Infrastructure.Persistence;

namespace VideoService.Infrastructure.Repositories;

public sealed class RecommendationFeedRepository(VideoDbContext context) : IRecommendationFeedRepository
{
    public async Task<IReadOnlyList<Video>> GetFeedAsync(int page, int pageSize, CancellationToken ct)
    => await context.Videos
        .Where(v => v.Status == VideoStatus.Published && v.Visibility == VideoVisibility.Public)
        .OrderByDescending(v => v.PublishedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);

    public async Task<IReadOnlyList<Video>> GetByTagOverlapAsync(IReadOnlyList<string> tags, VideoId? excludeVideoId, int limit, CancellationToken ct)
    {
        var candidates = await context.Videos
            .Where(v => v.Status == VideoStatus.Published && v.Visibility == VideoVisibility.Public)
            .Where(v => excludeVideoId == null || v.Id != excludeVideoId)
            .ToListAsync(ct);

        return candidates
            .Select(v => new { Video = v, Score = v.Tags.Count(t => tags.Contains(t.Value)) })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Video.ViewsCount)
            .Take(limit)
            .Select(x => x.Video)
            .ToList();
    }

    public async Task<IReadOnlyList<Video>> GetByUserPreferenceAsync(Guid userId, int limit, CancellationToken ct)
    {
        var recentLikedIds = await context.VideoReactions
            .Where(r => r.UserId == userId && r.Type == ReactionType.Like)
            .OrderByDescending(r => r.CreatedAt)
            .Take(20)
            .Select(r => r.VideoId)
            .ToListAsync(ct);

        if (recentLikedIds.Count == 0) return [];

        var likedVideos = await context.Videos.Where(v => recentLikedIds.Contains(v.Id)).ToListAsync(ct);
        var preferredTags = likedVideos.SelectMany(v => v.Tags.Select(t => t.Value)).Distinct().ToList();
        if (preferredTags.Count == 0) return [];

        var candidates = await context.Videos
            .Where(v => v.Status == VideoStatus.Published && !recentLikedIds.Contains(v.Id))
            .ToListAsync(ct);

        return candidates
            .Select(v => new { Video = v, Score = v.Tags.Count(t => preferredTags.Contains(t.Value)) })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Video.ViewsCount)
            .Take(limit)
            .Select(x => x.Video)
            .ToList();
    }

    public async Task<IReadOnlyList<Video>> GetTopViewedAsync(int limit, IReadOnlyCollection<VideoId> excludeIds, CancellationToken ct)
        => await context.Videos
            .Where(v => v.Status == VideoStatus.Published && v.Visibility == VideoVisibility.Public && !excludeIds.Contains(v.Id))
            .OrderByDescending(v => v.ViewsCount)
            .Take(limit)
            .ToListAsync(ct);
}