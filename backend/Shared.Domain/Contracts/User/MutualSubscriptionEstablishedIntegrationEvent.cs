namespace Shared.Domain.Contracts.User;

public sealed record MutualSubscriptionEstablishedIntegrationEvent(
    Guid UserAId,
    Guid UserBId,
    DateTime OccurredAt);