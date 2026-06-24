namespace Shared.Domain.Contracts.Auth;

public sealed record EmailConfirmedIntegrationEvent(
    Guid UserId,
    string Email,
    DateTime OccurredAt);