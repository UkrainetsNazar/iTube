using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;

namespace AuthService.Application.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Dto.Email!);
        if (emailResult.IsFailure)
            return Result.Failure<AuthResponseDto>(emailResult.Error);

        if (await userRepository.ExistsActiveByEmailAsync(emailResult.Value, cancellationToken))
            return Result.Failure<AuthResponseDto>(
                Error.Conflict("Register.EmailTaken", "Email is already taken."));

        var passwordHashResult = PasswordHash.Create(passwordHasher.Hash(request.Dto.Password!));
        if (passwordHashResult.IsFailure)
            return Result.Failure<AuthResponseDto>(passwordHashResult.Error);

        var newUser = User.Register(emailResult.Value, passwordHashResult.Value);

        await userRepository.AddAsync(newUser, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResponseDto(string.Empty, string.Empty));
    }
}