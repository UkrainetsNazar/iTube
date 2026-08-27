namespace VideoService.Domain.Entities;

public sealed class UserSubscription
{
    public Guid SubscriberId { get; private set; }
    public Guid ChannelId { get; private set; }
    public DateTime SubscribedAt { get; private set; }

    private UserSubscription() { }

    public UserSubscription(Guid subscriberId, Guid channelId, DateTime subscribedAt)
    {
        SubscriberId = subscriberId;
        ChannelId = channelId;
        SubscribedAt = subscribedAt;
    }
}