using MediatR;
using Shared.Domain.Common;

namespace AuthService.Application.Queries.GetUserByEmail;

public sealed record GetUserByEmailQuery(string Email) : IRequest<Result<UserLookupDto>>;

public sealed record UserLookupDto(Guid Id, string Email);