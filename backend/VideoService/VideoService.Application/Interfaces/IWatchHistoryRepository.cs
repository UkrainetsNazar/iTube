using Shared.Domain.ValueObjects;
using VideoService.Domain.Entities;

namespace VideoService.Application.Interfaces;

public interface IWatchHistoryRepository
{
    Task<WatchHistoryEntry?> FindAsync(Guid userId, VideoId videoId, CancellationToken ct);
    void Add(WatchHistoryEntry entry);
    void Update(WatchHistoryEntry entry);
    Task<(IReadOnlyList<WatchHistoryEntry> Items, int TotalCount)> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken ct);
}