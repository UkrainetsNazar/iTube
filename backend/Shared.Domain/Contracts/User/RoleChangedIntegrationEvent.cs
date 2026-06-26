namespace Shared.Domain.Contracts.User;

public sealed record RoleChangedIntegrationEvent(
    Guid UserId,
    string NewRole,
    DateTime OccurredAt);