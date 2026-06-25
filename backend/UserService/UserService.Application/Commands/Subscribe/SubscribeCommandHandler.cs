using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Events;

namespace UserService.Application.Commands.Subscribe;

public sealed class SubscribeCommandHandler(
    ISubscriptionRepository subscriptionRepository,
    IChannelRepository channelRepository,
    IUnitOfWork unitOfWork,
    IPublisher publisher
) : IRequestHandler<SubscribeCommand, Result>
{
    public async Task<Result> Handle(SubscribeCommand request, CancellationToken ct)
    {
        var subscriberId = UserId.From(request.SubscriberId);
        var targetChannelId = ChannelId.From(request.TargetChannelId);

        var targetChannel = await channelRepository.GetByIdAsync(targetChannelId, ct);
        if (targetChannel is null)
            return Result.Failure(Error.NotFound("Subscribe.ChannelNotFound", "Channel not found."));

        if (await subscriptionRepository.ExistsAsync(subscriberId, targetChannelId, ct))
            return Result.Failure(Error.Conflict("Subscribe.AlreadySubscribed", "Already subscribed."));

        var subscriptionResult = Subscription.Create(subscriberId, targetChannelId);
        if (subscriptionResult.IsFailure)
            return Result.Failure(subscriptionResult.Error);

        var subscription = subscriptionResult.Value;
        await subscriptionRepository.AddAsync(subscription, ct);

        var reverseSubscription = await subscriptionRepository.GetAsync(
            UserId.From(targetChannelId.Value),
            ChannelId.From(subscriberId.Value),
            ct);

        if (reverseSubscription is not null)
        {
            subscription.MarkAsMutual();
            reverseSubscription.MarkAsMutual();

            await publisher.Publish(
                new MutualSubscriptionEstablished(
                    UserId.From(request.SubscriberId),
                    UserId.From(targetChannelId.Value)),
                ct);
        }

        targetChannel.IncrementSubscribers();

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}