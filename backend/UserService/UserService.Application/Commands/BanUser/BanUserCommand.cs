using MediatR;
using Shared.Domain.Common;

namespace UserService.Application.Commands.BanUser;

public sealed record BanUserCommand(
    Guid UserId,
    Guid ModeratorId,
    string Reason,
    DateTime? ExpiresAt) : IRequest<Result>;