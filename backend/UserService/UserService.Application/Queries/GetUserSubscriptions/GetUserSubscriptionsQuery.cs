using MediatR;
using Shared.Domain.Common;

namespace UserService.Application.Queries.GetUserSubscriptions;

public sealed record GetUserSubscriptionsQuery(Guid UserId) : IRequest<Result<IReadOnlyList<SubscriptionDto>>>;

public sealed record SubscriptionDto(Guid ChannelId);