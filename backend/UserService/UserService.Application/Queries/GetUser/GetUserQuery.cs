using MediatR;
using Shared.Domain.Common;
using UserService.Application.DTOs;

namespace UserService.Application.Queries.GetUser;
 
public sealed record GetUserQuery(Guid UserId) : IRequest<Result<UserDto>>;