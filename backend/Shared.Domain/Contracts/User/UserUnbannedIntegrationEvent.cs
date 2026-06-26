namespace Shared.Domain.Contracts.User;

public sealed record UserUnbannedIntegrationEvent(
    Guid UserId,
    Guid? UnbannedByModeratorId,
    DateTime OccurredAt);