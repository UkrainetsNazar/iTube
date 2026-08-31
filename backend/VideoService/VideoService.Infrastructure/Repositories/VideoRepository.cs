using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Domain.Enums;
using VideoService.Infrastructure.Persistence;

namespace VideoService.Infrastructure.Repositories;

public sealed class VideoRepository(VideoDbContext context) : IVideoRepository
{
    public Task<Video?> GetByIdAsync(VideoId id, CancellationToken ct)
        => context.Videos.FirstOrDefaultAsync(v => v.Id == id, ct);

    public void Add(Video video) => context.Videos.Add(video);
    public void Update(Video video) => context.Videos.Update(video);

    public async Task<IReadOnlyList<Video>> GetByChannelAsync(Guid channelId, int page, int pageSize, CancellationToken ct)
        => await context.Videos
            .Where(v => v.AuthorId == channelId && v.Status == VideoStatus.Published && v.Visibility == VideoVisibility.Public)
            .OrderByDescending(v => v.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Video>> GetByChannelsAsync(IReadOnlyList<Guid> channelIds, int page, int pageSize, CancellationToken ct)
        => await context.Videos
            .Where(v => channelIds.Contains(v.AuthorId) && v.Status == VideoStatus.Published && v.Visibility == VideoVisibility.Public)
            .OrderByDescending(v => v.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Video>> GetByIdsAsync(IReadOnlyList<VideoId> ids, CancellationToken ct)
    => await context.Videos.Where(v => ids.Contains(v.Id)).ToListAsync(ct);

    public async Task<(IReadOnlyList<Video>, int)> GetAllByAuthorAsync(Guid authorId, int page, int pageSize, CancellationToken ct)
    {
        var query = context.Videos.Where(v => v.AuthorId == authorId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return (items, total);
    }
}