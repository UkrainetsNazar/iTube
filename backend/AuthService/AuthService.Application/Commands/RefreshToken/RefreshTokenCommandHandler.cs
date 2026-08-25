using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;

namespace AuthService.Application.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IPasswordHasher passwordHasher, IUserRepository userRepository, IUnitOfWork unitOfWork, IJwtGenerator jwtGenerator)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = passwordHasher.HashToken(request.RefreshToken);
        var user = await userRepository.GetByRefreshTokenAsync(tokenHash, cancellationToken);

        if (user == null)
            return Result.Failure<AuthResponseDto>(Error.Failure("Refresh.UserNotFound", "There is no user with this token"));

        var revokeResult = user.RevokeRefreshToken(tokenHash);
        if (revokeResult.IsFailure)
            return Result.Failure<AuthResponseDto>(revokeResult.Error);

        var accessToken = jwtGenerator.GenerateAccessToken(user);
        var refreshTokenRaw = jwtGenerator.GenerateRefreshToken();

        user.IssueRefreshToken(
            tokenHash: passwordHasher.HashToken(refreshTokenRaw),
            expiresAt: DateTime.UtcNow.AddDays(7));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(new AuthResponseDto(accessToken, refreshTokenRaw));
    }
}