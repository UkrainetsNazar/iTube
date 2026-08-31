namespace UserService.Application.DTOs;

public sealed record UserAccountAdminRow(
    Guid UserId, string ChannelName, string Role, string Status,
    bool IsBanned, string? BanReason, DateTime? BanExpiresAt,
    int SubscribersCount, DateTime CreatedAt);