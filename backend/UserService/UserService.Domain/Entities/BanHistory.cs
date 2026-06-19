using Shared.Domain.ValueObjects;

namespace UserService.Domain.Entities;

public sealed class BanHistory
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public UserId UserId { get; private set; }
    public UserId BannedByModeratorId { get; private set; }

    public string Reason { get; private set; } = string.Empty;
    public DateTime BannedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? UnbannedAt { get; private set; }
    public UserId? UnbannedByModeratorId { get; private set; }

    private BanHistory()
    {
    }

    internal static BanHistory FromBan(Ban ban)
    {
        return new BanHistory
        {
            UserId = ban.UserId,
            BannedByModeratorId = ban.BannedByModeratorId,
            Reason = ban.Reason,
            BannedAt = ban.BannedAt,
            ExpiresAt = ban.ExpiresAt
        };
    }

    internal void RecordUnban(UserId? unbannedBy)
    {
        UnbannedAt = DateTime.UtcNow;
        UnbannedByModeratorId = unbannedBy;
    }
}