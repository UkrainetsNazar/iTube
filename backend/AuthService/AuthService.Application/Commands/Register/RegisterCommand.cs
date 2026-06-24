using AuthService.Application.DTOs;
using MediatR;
using Shared.Domain.Common;

namespace AuthService.Application.Commands.Register;

public sealed record RegisterCommand(AuthDto Dto) : IRequest<Result<AuthResponseDto>>;