using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;

namespace UserService.Application.Commands.Unsubscribe;

public sealed class UnsubscribeCommandHandler(
    ISubscriptionRepository subscriptionRepository,
    IChannelRepository channelRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UnsubscribeCommand, Result>
{
    public async Task<Result> Handle(UnsubscribeCommand request, CancellationToken ct)
    {
        var subscriberId = UserId.From(request.SubscriberId);
        var targetChannelId = ChannelId.From(request.TargetChannelId);

        var subscription = await subscriptionRepository.GetAsync(subscriberId, targetChannelId, ct);
        if (subscription is null)
            return Result.Failure(Error.NotFound("Unsubscribe.NotFound", "Subscription not found."));

        var result = subscription.Unsubscribe();
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await subscriptionRepository.RemoveAsync(subscription, ct);

        var reverseSubscription = await subscriptionRepository.GetAsync(
            UserId.From(targetChannelId.Value),
            ChannelId.From(subscriberId.Value),
            ct);

        reverseSubscription?.UnmarkAsMutual();

        var targetChannel = await channelRepository.GetByIdAsync(targetChannelId, ct);
        targetChannel?.DecrementSubscribers();

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}