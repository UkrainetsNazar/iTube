using MediatR;
using Shared.Domain.Common;

namespace AuthService.Application.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;