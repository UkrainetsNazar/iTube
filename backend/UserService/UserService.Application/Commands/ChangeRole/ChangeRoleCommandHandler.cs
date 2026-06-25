using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;

namespace UserService.Application.Commands.ChangeRole;

public sealed class ChangeRoleCommandHandler(
    IUserAccountRepository userAccountRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<ChangeRoleCommand, Result>
{
    public async Task<Result> Handle(ChangeRoleCommand request, CancellationToken ct)
    {
        var userId = UserId.From(request.UserId);
        var userAccount = await userAccountRepository.GetByIdAsync(userId, ct);

        if (userAccount is null)
            return Result.Failure(Error.NotFound("ChangeRole.UserNotFound", "User not found."));

        var result = userAccount.ChangeRole(request.NewRole, UserId.From(request.ChangedBy));
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}