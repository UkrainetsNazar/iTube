using MediatR;
using Shared.Domain.Common;

namespace AuthService.Application.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Token) : IRequest<Result>;