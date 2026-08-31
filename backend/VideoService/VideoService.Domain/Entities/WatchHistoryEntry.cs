using Shared.Domain.ValueObjects;

namespace VideoService.Domain.Entities;

public sealed class WatchHistoryEntry
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public VideoId VideoId { get; private set; } = null!;
    public DateTime WatchedAt { get; private set; }

    private WatchHistoryEntry() { }

    public WatchHistoryEntry(Guid userId, VideoId videoId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        VideoId = videoId;
        WatchedAt = DateTime.UtcNow;
    }

    public void Touch() => WatchedAt = DateTime.UtcNow;
}