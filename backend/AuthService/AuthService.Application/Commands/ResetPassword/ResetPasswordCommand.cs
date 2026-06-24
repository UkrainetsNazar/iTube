using MediatR;
using Shared.Domain.Common;

namespace AuthService.Application.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : IRequest<Result>;