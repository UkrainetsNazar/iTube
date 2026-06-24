using AuthService.Application.Interfaces;
using AuthService.Domain.ValueObjects;
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;

namespace AuthService.Application.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Error);

        var user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null)
            return Result.Failure(Error.NotFound("ResetPassword.UserNotFound", "User not found."));

        var passwordHashResult = PasswordHash.Create(passwordHasher.Hash(request.NewPassword));
        if (passwordHashResult.IsFailure)
            return Result.Failure(passwordHashResult.Error);

        var tokenHash = passwordHasher.Hash(request.Token);

        var resetResult = user.ResetPassword(tokenHash, passwordHashResult.Value);
        if (resetResult.IsFailure)
            return Result.Failure(resetResult.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}