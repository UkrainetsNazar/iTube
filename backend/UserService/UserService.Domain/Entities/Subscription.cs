using Shared.Domain.Common;
using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;
using UserService.Domain.Events;

namespace UserService.Domain.Entities;

public sealed class Subscription : AggregateRoot<Guid>
{
    public UserId SubscriberId { get; private set; }
    public ChannelId TargetChannelId { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsMutual { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Subscription()
    {
    }

    private Subscription(UserId subscriberId, ChannelId targetChannelId) : base(Guid.NewGuid())
    {
        SubscriberId = subscriberId;
        TargetChannelId = targetChannelId;
        IsActive = true;
        IsMutual = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Subscription> Create(UserId subscriberId, ChannelId targetChannelId)
    {
        if (subscriberId.Value == targetChannelId.Value)
            return Result.Failure<Subscription>(Error.Validation(
                "Subscription.SelfSubscribe", "It is not possible to subscribe to your own channel."));

        var subscription = new Subscription(subscriberId, targetChannelId);
        subscription.RaiseDomainEvent(new UserSubscribed(subscriberId, targetChannelId));
        return subscription;
    }

    public void MarkAsMutual() => IsMutual = true;

    public void UnmarkAsMutual() => IsMutual = false;

    public Result Unsubscribe()
    {
        if (!IsActive)
            return Result.Failure(Error.Conflict("Subscription.AlreadyInactive", "The subscription is no longer active."));

        IsActive = false;
        IsMutual = false;
        RaiseDomainEvent(new UserUnsubscribed(SubscriberId, TargetChannelId));
        return Result.Success();
    }
}