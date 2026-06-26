namespace Shared.Domain.Contracts.User;

public sealed record UserBannedIntegrationEvent(
    Guid UserId,
    Guid ModeratorId,
    string Reason,
    DateTime? ExpiresAt,
    DateTime OccurredAt);