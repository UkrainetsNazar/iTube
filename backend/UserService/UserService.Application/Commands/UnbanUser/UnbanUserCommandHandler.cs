using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;

namespace UserService.Application.Commands.UnbanUser;

public sealed class UnbanUserCommandHandler(
    IUserAccountRepository userAccountRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UnbanUserCommand, Result>
{
    public async Task<Result> Handle(UnbanUserCommand request, CancellationToken ct)
    {
        var userAccount = await userAccountRepository.GetByIdAsync(UserId.From(request.UserId), ct);

        if (userAccount is null)
            return Result.Failure(Error.NotFound("UnbanUser.UserNotFound", "User not found."));

        var result = userAccount.Unban(UserId.From(request.ModeratorId));
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}