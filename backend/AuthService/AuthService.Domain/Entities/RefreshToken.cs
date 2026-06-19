using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace AuthService.Domain.Entities;

public sealed class RefreshToken : Entity<Guid>
{
    public UserId UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => RevokedAt is null && !IsExpired;

    private RefreshToken()
    {
    }

    internal RefreshToken(UserId userId, string tokenHash, DateTime expiresAt)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    internal void Revoke() => RevokedAt ??= DateTime.UtcNow;
}