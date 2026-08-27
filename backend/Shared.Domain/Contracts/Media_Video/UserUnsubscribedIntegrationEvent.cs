namespace Shared.Domain.Contracts.Media_Video;

public sealed record UserUnsubscribedIntegrationEvent(Guid SubscriberId, Guid ChannelId);