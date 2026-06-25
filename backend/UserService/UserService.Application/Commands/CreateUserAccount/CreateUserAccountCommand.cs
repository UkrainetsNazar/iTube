using MediatR;
using Shared.Domain.Common;

namespace UserService.Application.Commands.CreateUserAccount;

public sealed record CreateUserAccountCommand(Guid UserId, string Email) : IRequest<Result>;