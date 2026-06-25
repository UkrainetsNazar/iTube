using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Commands.CreateUserAccount;

public sealed class CreateUserAccountCommandHandler(
    IUserAccountRepository userAccountRepository,
    IChannelRepository channelRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateUserAccountCommand, Result>
{
    public async Task<Result> Handle(CreateUserAccountCommand request, CancellationToken ct)
    {
        var userId = UserId.From(request.UserId);

        if (await userAccountRepository.ExistsAsync(userId, ct))
            return Result.Success();

        var userAccount = UserAccount.Create(userId);
        await userAccountRepository.AddAsync(userAccount, ct);

        var channelNameResult = ChannelName.Create(request.Email.Split('@')[0]);
        if (channelNameResult.IsFailure)
            return Result.Failure(channelNameResult.Error);

        var channel = Channel.Create(userId, channelNameResult.Value);
        await channelRepository.AddAsync(channel, ct);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}