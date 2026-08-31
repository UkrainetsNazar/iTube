// VideoService.Infrastructure/Repositories/WatchHistoryRepository.cs
using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Infrastructure.Persistence;

namespace VideoService.Infrastructure.Repositories;

public sealed class WatchHistoryRepository(VideoDbContext context) : IWatchHistoryRepository
{
    public Task<WatchHistoryEntry?> FindAsync(Guid userId, VideoId videoId, CancellationToken ct)
        => context.WatchHistory.FirstOrDefaultAsync(w => w.UserId == userId && w.VideoId == videoId, ct);

    public void Add(WatchHistoryEntry entry) => context.WatchHistory.Add(entry);
    public void Update(WatchHistoryEntry entry) => context.WatchHistory.Update(entry);

    public async Task<(IReadOnlyList<WatchHistoryEntry>, int)> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken ct)
    {
        var query = context.WatchHistory.Where(w => w.UserId == userId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(w => w.WatchedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return (items, total);
    }
}