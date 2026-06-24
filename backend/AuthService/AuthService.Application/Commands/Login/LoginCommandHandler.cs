using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.ValueObjects;
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;

namespace AuthService.Application.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtGenerator jwtGenerator,
    IUnitOfWork unitOfWork
) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Dto.Email!);
        if (emailResult.IsFailure)
            return Result.Failure<AuthResponseDto>(emailResult.Error);

        var user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null)
            return Result.Failure<AuthResponseDto>(
                Error.Failure("Login.Failed", "Invalid email or password."));

        if (!passwordHasher.Verify(request.Dto.Password!, user.PasswordHash.Value))
            return Result.Failure<AuthResponseDto>(
                Error.Failure("Login.Failed", "Invalid email or password."));

        var loginResult = user.Login();
        if (loginResult.IsFailure)
            return Result.Failure<AuthResponseDto>(loginResult.Error);

        var accessToken = jwtGenerator.GenerateAccessToken(user);
        var refreshTokenRaw = jwtGenerator.GenerateRefreshToken();

        user.IssueRefreshToken(
            tokenHash: passwordHasher.Hash(refreshTokenRaw),
            expiresAt: DateTime.UtcNow.AddDays(7));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResponseDto(
            AccessToken: accessToken,
            RefreshToken: refreshTokenRaw));
    }
}