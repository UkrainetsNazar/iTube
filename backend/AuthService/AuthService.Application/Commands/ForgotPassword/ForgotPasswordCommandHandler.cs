using AuthService.Application.Interfaces;
using AuthService.Domain.ValueObjects;
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;

namespace AuthService.Application.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<ForgotPasswordCommand, Result>
{
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Error);

        var user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null)
            return Result.Success();

        var rawToken = Guid.NewGuid().ToString("N");
        var tokenHash = passwordHasher.Hash(rawToken);

        user.GeneratePasswordResetToken(tokenHash, rawToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}