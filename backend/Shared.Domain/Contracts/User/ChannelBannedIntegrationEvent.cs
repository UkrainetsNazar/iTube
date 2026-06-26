namespace Shared.Domain.Contracts.User;

public sealed record ChannelBannedIntegrationEvent(
    Guid ChannelId,
    string Reason,
    DateTime OccurredAt);