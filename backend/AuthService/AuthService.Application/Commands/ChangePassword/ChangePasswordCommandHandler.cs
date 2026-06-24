using AuthService.Application.Interfaces;
using AuthService.Domain.ValueObjects;
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;

namespace AuthService.Application.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Error);

        var user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null)
            return Result.Failure(
                Error.Failure("Login.Failed", "Invalid email or password."));

        if (!passwordHasher.Verify(request.OldPassword!, user.PasswordHash.Value))
            return Result.Failure(
                Error.Failure("Login.Failed", "Invalid email or password."));

        var newPasswordHash = PasswordHash.Create(request.NewPassword);
        if (newPasswordHash.IsFailure)
            return Result.Failure(newPasswordHash.Error);

        user.ChangePassword(newPasswordHash.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}