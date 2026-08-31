using AuthService.Application.Interfaces;
using AuthService.Domain.ValueObjects;
using MediatR;
using Shared.Domain.Common;

namespace AuthService.Application.Queries.GetUserByEmail;

public sealed class GetUserByEmailQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByEmailQuery, Result<UserLookupDto>>
{
    public async Task<Result<UserLookupDto>> Handle(GetUserByEmailQuery request, CancellationToken ct)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<UserLookupDto>(emailResult.Error);

        var user = await userRepository.GetByEmailAsync(emailResult.Value, ct);
        if (user is null)
            return Result.Failure<UserLookupDto>(Error.NotFound("User.NotFound", "User not found."));

        return Result.Success(new UserLookupDto(user.Id.Value, user.Email.Value));
    }
}