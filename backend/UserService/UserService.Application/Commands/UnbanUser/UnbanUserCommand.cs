using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;

namespace UserService.Application.Commands.UnbanUser;

public sealed record UnbanUserCommand(Guid UserId, Guid ModeratorId) : IRequest<Result>;