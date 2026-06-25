using MediatR;
using Shared.Domain.Common;
using UserService.Domain.Enums;

namespace UserService.Application.Commands.ChangeRole;

public sealed record ChangeRoleCommand(Guid UserId, Guid ChangedBy, UserRole NewRole) : IRequest<Result>;