using AuthService.Application.Interfaces;
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;

namespace AuthService.Application.Commands.Logout;

public sealed class LogoutCommandHandler(
    IPasswordHasher passwordHasher, IUserRepository userRepository, IUnitOfWork unitOfWork) 
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = passwordHasher.HashToken(request.RefreshToken);
        var user = await userRepository.GetByRefreshTokenAsync(tokenHash, cancellationToken);

        if(user == null)
            return Result.Failure(Error.Failure("Logout.UserNotFound","There is no user with this token"));

        var revokeResult = user.RevokeRefreshToken(tokenHash);
        if (revokeResult.IsFailure)
            return Result.Failure(revokeResult.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}