namespace AuthService.Domain.Entities;

public sealed class PasswordResetToken
{
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid => !IsUsed && !IsExpired;

    private PasswordResetToken() { }

    internal PasswordResetToken(string tokenHash, DateTime expiresAt)
    {
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        IsUsed = false;
    }

    internal void MarkAsUsed() => IsUsed = true;
}