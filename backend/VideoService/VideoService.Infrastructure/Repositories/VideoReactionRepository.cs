using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Domain.Enums;
using VideoService.Infrastructure.Persistence;

namespace VideoService.Infrastructure.Repositories;

public sealed class VideoReactionRepository(VideoDbContext context) : IVideoReactionRepository
{
    public Task<VideoReaction?> GetByVideoAndUserAsync(VideoId videoId, Guid userId, CancellationToken ct)
        => context.VideoReactions.FirstOrDefaultAsync(r => r.VideoId == videoId && r.UserId == userId, ct);

    public void Add(VideoReaction reaction) => context.VideoReactions.Add(reaction);
    public void Update(VideoReaction reaction) => context.VideoReactions.Update(reaction);
    public void Remove(VideoReaction reaction) => context.VideoReactions.Remove(reaction);
    
    public async Task<(IReadOnlyList<VideoId>, int)> GetLikedVideoIdsByUserAsync(Guid userId, int page, int pageSize, CancellationToken ct)
    {
        var query = context.VideoReactions.Where(r => r.UserId == userId && r.Type == ReactionType.Like);
        var total = await query.CountAsync(ct);
        var ids = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => r.VideoId)
            .ToListAsync(ct);
        return (ids, total);
    }
}