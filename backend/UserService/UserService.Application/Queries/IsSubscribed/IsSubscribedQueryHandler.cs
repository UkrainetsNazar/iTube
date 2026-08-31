using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;

namespace UserService.Application.Queries.IsSubscribed;

public sealed class IsSubscribedQueryHandler(ISubscriptionRepository subscriptionRepository)
    : IRequestHandler<IsSubscribedQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(IsSubscribedQuery request, CancellationToken ct)
    {
        var exists = await subscriptionRepository.ExistsAsync(
            UserId.From(request.SubscriberId), ChannelId.From(request.ChannelId), ct);
        return Result.Success(exists);
    }
}