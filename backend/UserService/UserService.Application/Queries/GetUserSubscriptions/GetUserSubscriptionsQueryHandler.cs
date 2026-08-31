using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;

namespace UserService.Application.Queries.GetUserSubscriptions;

public sealed class GetUserSubscriptionsQueryHandler(ISubscriptionRepository subscriptionRepository)
    : IRequestHandler<GetUserSubscriptionsQuery, Result<IReadOnlyList<SubscriptionDto>>>
{
    public async Task<Result<IReadOnlyList<SubscriptionDto>>> Handle(GetUserSubscriptionsQuery request, CancellationToken ct)
    {
        var subscriptions = await subscriptionRepository.GetBySubscriberIdAsync(UserId.From(request.UserId), ct);
        return Result.Success<IReadOnlyList<SubscriptionDto>>(
            subscriptions.Select(s => new SubscriptionDto(s.TargetChannelId.Value)).ToList());
    }
}