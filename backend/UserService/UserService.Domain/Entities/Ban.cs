using Shared.Domain.ValueObjects;

namespace UserService.Domain.Entities;

public sealed class Ban
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public UserId UserId { get; private set; }
    public UserId BannedByModeratorId { get; private set; }

    public string Reason { get; private set; } = string.Empty;
    public DateTime BannedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; private set; }

    public bool IsPermanent => ExpiresAt is null;
    public bool IsActive => IsPermanent || ExpiresAt > DateTime.UtcNow;

    private Ban()
    {
    }

    internal Ban(UserId userId, UserId bannedByModeratorId, string reason, DateTime? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("The reason for the ban cannot be blank.", nameof(reason));

        if (expiresAt is not null && expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("ExpiresAt must be in the future.", nameof(expiresAt));

        UserId = userId;
        BannedByModeratorId = bannedByModeratorId;
        Reason = reason;
        ExpiresAt = expiresAt;
    }
}