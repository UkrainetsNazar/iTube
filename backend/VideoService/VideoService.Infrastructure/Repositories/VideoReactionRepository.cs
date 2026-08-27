using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Infrastructure.Persistence;

namespace VideoService.Infrastructure.Repositories;

public sealed class VideoReactionRepository(VideoDbContext context) : IVideoReactionRepository
{
    public Task<VideoReaction?> GetByVideoAndUserAsync(VideoId videoId, Guid userId, CancellationToken ct)
        => context.VideoReactions.FirstOrDefaultAsync(r => r.VideoId == videoId && r.UserId == userId, ct);

    public void Add(VideoReaction reaction) => context.VideoReactions.Add(reaction);
    public void Update(VideoReaction reaction) => context.VideoReactions.Update(reaction);
    public void Remove(VideoReaction reaction) => context.VideoReactions.Remove(reaction);
}