using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.Queries.GetUser;

public sealed class GetUserQueryHandler(IUserAccountRepository userAccountRepository)
    : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken ct)
    {
        var user = await userAccountRepository.GetByIdAsync(UserId.From(request.UserId), ct);
 
        if (user is null)
            return Result.Failure<UserDto>(Error.NotFound("GetUser.NotFound", "User not found."));
 
        return new UserDto(
            user.Id.Value,
            user.Role.ToString(),
            user.Status.ToString(),
            user.CreatedAt);
    }
}