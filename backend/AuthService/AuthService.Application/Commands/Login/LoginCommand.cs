using AuthService.Application.DTOs;
using MediatR;
using Shared.Domain.Common;

namespace AuthService.Application.Commands.Login;

public sealed record LoginCommand(AuthDto Dto) : IRequest<Result<AuthResponseDto>>;