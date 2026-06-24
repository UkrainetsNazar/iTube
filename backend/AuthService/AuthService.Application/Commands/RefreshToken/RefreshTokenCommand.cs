using AuthService.Application.DTOs;
using MediatR;
using Shared.Domain.Common;

namespace AuthService.Application.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponseDto>>;