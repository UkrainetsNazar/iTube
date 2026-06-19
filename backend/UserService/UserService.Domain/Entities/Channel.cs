using Shared.Common;
using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;
using UserService.Domain.Enums;
using UserService.Domain.Events;
using UserService.Domain.ValueObjects;

namespace UserService.Domain.Entities;

public sealed class Channel : AggregateRoot<ChannelId>
{
    public UserId OwnerId { get; private set; }
    public ChannelName Name { get; private set; } = null!;
    public MediaReference? Avatar { get; private set; }
    public MediaReference? Banner { get; private set; }
    public ChannelDescription? Description { get; private set; }
    public int SubscribersCount { get; private set; }
    public int VideoCount { get; private set; }
    public ChannelStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Channel()
    {
    }

    private Channel(ChannelId id, UserId ownerId, ChannelName name) : base(id)
    {
        OwnerId = ownerId;
        Name = name;
        Status = ChannelStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public static Channel Create(UserId ownerId, ChannelName name)
    {
        var channel = new Channel(ChannelId.From(ownerId.Value), ownerId, name);
        channel.RaiseDomainEvent(new ChannelCreated(channel.Id, ownerId));
        return channel;
    }

    public Result UpdateProfile(ChannelName name, ChannelDescription? description, MediaReference? avatar, MediaReference? banner)
    {
        if (Status == ChannelStatus.Banned)
            return Result.Failure(Error.Conflict("Channel.Banned", "A banned channel cannot be edited."));

        Name = name;
        Description = description;
        Avatar = avatar;
        Banner = banner;
        return Result.Success();
    }

    public void IncrementSubscribers() => SubscribersCount++;

    public void DecrementSubscribers() => SubscribersCount = Math.Max(0, SubscribersCount - 1);

    public void IncrementVideoCount() => VideoCount++;

    public void DecrementVideoCount() => VideoCount = Math.Max(0, VideoCount - 1);

    public Result Ban(string reason)
    {
        if (Status == ChannelStatus.Banned)
            return Result.Failure(Error.Conflict("Channel.AlreadyBanned", "The channel has already been banned."));

        Status = ChannelStatus.Banned;
        RaiseDomainEvent(new ChannelBanned(Id, reason));
        return Result.Success();
    }

    public Result Unban()
    {
        if (Status == ChannelStatus.Active)
            return Result.Failure(Error.Conflict("Channel.NotBanned", "The channel has not been banned."));

        Status = ChannelStatus.Active;
        RaiseDomainEvent(new ChannelUnbanned(Id));
        return Result.Success();
    }
}