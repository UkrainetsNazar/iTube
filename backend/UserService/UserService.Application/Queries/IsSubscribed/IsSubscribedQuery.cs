using MediatR;
using Shared.Domain.Common;

namespace UserService.Application.Queries.IsSubscribed;

public sealed record IsSubscribedQuery(Guid SubscriberId, Guid ChannelId) : IRequest<Result<bool>>;