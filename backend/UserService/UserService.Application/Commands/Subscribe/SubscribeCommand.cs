using MediatR;
using Shared.Domain.Common;

namespace UserService.Application.Commands.Subscribe;

public sealed record SubscribeCommand(Guid SubscriberId, Guid TargetChannelId) : IRequest<Result>;