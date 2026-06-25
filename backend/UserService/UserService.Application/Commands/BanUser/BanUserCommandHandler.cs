using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;

namespace UserService.Application.Commands.BanUser;

public sealed class BanUserCommandHandler(
    IUserAccountRepository userAccountRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<BanUserCommand, Result>
{
    public async Task<Result> Handle(BanUserCommand request, CancellationToken ct)
    {
        var userAccount = await userAccountRepository.GetByIdAsync(UserId.From(request.UserId), ct);

        if (userAccount is null)
            return Result.Failure(Error.NotFound("BanUser.UserNotFound", "User not found."));

        var result = userAccount.Ban(request.Reason, request.ExpiresAt, UserId.From(request.ModeratorId));
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}