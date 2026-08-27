namespace Shared.Domain.Contracts.Media_Video;

public sealed record UserSubscribedIntegrationEvent(Guid SubscriberId, Guid ChannelId, DateTime SubscribedAt);