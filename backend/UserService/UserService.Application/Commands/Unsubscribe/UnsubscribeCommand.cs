using MediatR;
using Shared.Domain.Common;

namespace UserService.Application.Commands.Unsubscribe;

public sealed record UnsubscribeCommand(Guid SubscriberId, Guid TargetChannelId) : IRequest<Result>;