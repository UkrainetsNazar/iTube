using Shared.Common;
using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;
using UserService.Domain.Enums;
using UserService.Domain.Events;

namespace UserService.Domain.Entities;

public sealed class UserAccount : AggregateRoot<UserId>
{
    public UserRole Role { get; private set; }
    public UserAccountStatus Status { get; private set; }

    public Ban? CurrentBan { get; private set; }

    private readonly List<BanHistory> _history = [];
    public IReadOnlyList<BanHistory> History => _history.AsReadOnly();

    public DateTime CreatedAt { get; private set; }

    private UserAccount()
    {
    }

    private UserAccount(UserId id) : base(id)
    {
        Role = UserRole.User;
        Status = UserAccountStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public static UserAccount Create(UserId userId) => new(userId);

    public Result ChangeRole(UserRole newRole, UserId changedBy)
    {
        if (Status == UserAccountStatus.Deleted)
            return Result.Failure(Error.Conflict("UserAccount.Deleted", "Account deleted."));

        if (Role == newRole)
            return Result.Success();

        Role = newRole;
        RaiseDomainEvent(new RoleChanged(Id, newRole, changedBy));
        return Result.Success();
    }

    public Result Ban(string reason, DateTime? expiresAt, UserId moderatorId)
    {
        if (CurrentBan is { IsActive: true })
            return Result.Failure(Error.Conflict("UserAccount.AlreadyBanned", "The user has already been banned."));

        try
        {
            CurrentBan = new Ban(Id, moderatorId, reason, expiresAt);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(Error.Validation("UserAccount.InvalidBan", ex.Message));
        }

        Status = UserAccountStatus.Banned;

        RaiseDomainEvent(new UserBanned(Id, moderatorId, reason, expiresAt));
        return Result.Success();
    }

    public Result Unban(UserId? unbannedByModeratorId)
    {
        if (CurrentBan is null)
            return Result.Failure(Error.Conflict("UserAccount.NotBanned", "The user is not banned."));

        var historyEntry = BanHistory.FromBan(CurrentBan);
        historyEntry.RecordUnban(unbannedByModeratorId);
        _history.Add(historyEntry);

        CurrentBan = null;
        Status = UserAccountStatus.Active;

        RaiseDomainEvent(new UserUnbanned(Id, unbannedByModeratorId));
        return Result.Success();
    }

    public void ExpireBanIfNeeded()
    {
        if (CurrentBan is { IsPermanent: false, IsActive: false })
            Unban(unbannedByModeratorId: null);
    }
}