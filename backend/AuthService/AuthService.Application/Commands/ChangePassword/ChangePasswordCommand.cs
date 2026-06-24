using MediatR;
using Shared.Domain.Common;

namespace AuthService.Application.Commands.ChangePassword;

public sealed record ChangePasswordCommand(string Email, string OldPassword, string NewPassword) : IRequest<Result>;