using MediatR;
using Shared.Domain.Common;

namespace AuthService.Application.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;