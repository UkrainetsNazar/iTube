using AuthService.Domain.Enums;
using AuthService.Domain.Events;
using AuthService.Domain.ValueObjects;
using Shared.Common;
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
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

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
            EmailConfirmationToken = Guid.NewGuid().ToString("N")
        };

        user.RaiseDomainEvent(new UserRegistered(user.Id, user.Email.Value));
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

        RaiseDomainEvent(new EmailConfirmed(Id));
        return Result.Success();
    }

    public Result Login()
    {
        if (Status != AccountStatus.Active)
            return Result.Failure(Error.Conflict("User.NotActive", $"Login denied: account status — {Status}."));

        LastLoginAt = DateTime.UtcNow;
        RaiseDomainEvent(new UserLoggedIn(Id));
        return Result.Success();
    }

    public void ChangePassword(PasswordHash newPasswordHash) => PasswordHash = newPasswordHash;

    public RefreshToken IssueRefreshToken(string tokenHash, DateTime expiresAt)
    {
        var token = new RefreshToken(Id, tokenHash, expiresAt);
        _refreshTokens.Add(token);
        return token;
    }

    public Result RevokeRefreshToken(Guid refreshTokenId)
    {
        var token = _refreshTokens.FirstOrDefault(t => t.Id == refreshTokenId);
        if (token is null)
            return Result.Failure(Error.NotFound("RefreshToken.NotFound", "Refresh-token was not founded."));

        token.Revoke();
        return Result.Success();
    }

    public void ApplyRoleChanged(UserRole newRole) => Role = newRole;

    public void ApplyBan() => Status = AccountStatus.Banned;

    public void ApplyUnban() =>
        Status = IsEmailConfirmed ? AccountStatus.Active : AccountStatus.PendingConfirmation;
}