using AuthService.Application.Interfaces;
using AuthService.Domain.ValueObjects;
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;

namespace AuthService.Application.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Error);

        var user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null)
            return Result.Failure(Error.NotFound("ConfirmEmail.UserNotFound", "User not found."));

        var confirmResult = user.ConfirmEmail(request.Token);
        if (confirmResult.IsFailure)
            return Result.Failure(confirmResult.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}