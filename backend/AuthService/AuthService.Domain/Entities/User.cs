using AuthService.Domain.Enums;
using AuthService.Domain.Events;
using AuthService.Domain.ValueObjects;
using Shared.Domain.Common;
using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace AuthService.Domain.Entities;

public sealed class User : AggregateRoot<UserId>
{
    public Email Email { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public AccountStatus Status { get; private set; }
    public bool IsEmailConfirmed { get; private set; }
    public string? EmailConfirmationToken { get; private set; }
    public DateTime? EmailConfirmationTokenExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private PasswordResetToken? _passwordResetToken;
    public PasswordResetToken? PasswordResetToken => _passwordResetToken;

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User()
    {
    }

    private User(UserId id, Email email, PasswordHash passwordHash) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = UserRole.User;
        Status = AccountStatus.PendingConfirmation;
        IsEmailConfirmed = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Register(Email email, PasswordHash passwordHash)
    {
        var user = new User(UserId.New(), email, passwordHash)
        {
            EmailConfirmationToken = Guid.NewGuid().ToString("N"),
            EmailConfirmationTokenExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id, user.Email.Value));
        return user;
    }

    public Result ConfirmEmail(string token)
    {
        if (IsEmailConfirmed)
            return Result.Failure(Error.Conflict("User.AlreadyConfirmed", "Email has already been confirmed."));

        if (EmailConfirmationToken is null || EmailConfirmationToken != token)
            return Result.Failure(Error.Validation("User.InvalidToken", "The verification token is invalid."));

        IsEmailConfirmed = true;
        EmailConfirmationToken = null;
        Status = AccountStatus.Active;

        RaiseDomainEvent(new EmailConfirmedDomainEvent(Id, Email.Value));
        return Result.Success();
    }

    public Result Login()
    {
        if (Status != AccountStatus.Active)
            return Result.Failure(Error.Conflict("User.NotActive", $"Login denied: account status — {Status}."));

        LastLoginAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void ChangePassword(PasswordHash newPasswordHash) => PasswordHash = newPasswordHash;

    public void GeneratePasswordResetToken(string tokenHash, string rawToken)
    {
        _passwordResetToken = new PasswordResetToken(tokenHash, DateTime.UtcNow.AddHours(1));
        RaiseDomainEvent(new PasswordResetRequestedDomainEvent(Id, Email.Value, rawToken));
    }

    public Result ResetPassword(string tokenHash, PasswordHash newPasswordHash)
    {
        if (_passwordResetToken is null || !_passwordResetToken.IsValid)
            return Result.Failure(Error.Validation("User.InvalidResetToken", "Reset token is invalid or expired."));

        if (_passwordResetToken.TokenHash != tokenHash)
            return Result.Failure(Error.Validation("User.InvalidResetToken", "Reset token is invalid or expired."));

        PasswordHash = newPasswordHash;
        _passwordResetToken.MarkAsUsed();
        return Result.Success();
    }

    public RefreshToken IssueRefreshToken(string tokenHash, DateTime expiresAt)
    {
        var token = new RefreshToken(Id, tokenHash, expiresAt);
        _refreshTokens.Add(token);
        return token;
    }

    public Result RevokeRefreshToken(string tokenHash)
    {
        var token = _refreshTokens.FirstOrDefault(t => t.TokenHash == tokenHash);
        if (token is null)
            return Result.Failure(Error.NotFound("RefreshToken.NotFound", "Token not found."));

        if (!token.IsActive)
            return Result.Failure(Error.Conflict("RefreshToken.NotActive", "Token is already revoked or expired."));

        token.Revoke();
        return Result.Success();
    }

    public void ApplyRoleChanged(UserRole newRole) => Role = newRole;

    public void ApplyBan() => Status = AccountStatus.Banned;

    public void ApplyUnban() =>
        Status = IsEmailConfirmed ? AccountStatus.Active : AccountStatus.PendingConfirmation;
}