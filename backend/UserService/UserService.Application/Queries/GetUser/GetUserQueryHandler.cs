using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.Queries.GetUser;

public sealed class GetUserQueryHandler(
    IUserAccountRepository userAccountRepository,
    IChannelRepository channelRepository) : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken ct)
    {
        var userId = UserId.From(request.UserId);
        var user = await userAccountRepository.GetByIdAsync(userId, ct);

        if (user is null)
            return Result.Failure<UserDto>(Error.NotFound("GetUser.NotFound", "User not found."));

        var channel = await channelRepository.GetByOwnerIdAsync(userId, ct);

        var channelDto = channel is null ? null : new ChannelSummaryDto(
            channel.Id.Value, channel.Name.Value, channel.Description?.Value,
            channel.SubscribersCount, channel.VideoCount, channel.Status.ToString());

        var currentBanDto = user.CurrentBan is null ? null : new BanDto(
            user.CurrentBan.Id, user.CurrentBan.BannedByModeratorId.Value, user.CurrentBan.Reason,
            user.CurrentBan.BannedAt, user.CurrentBan.ExpiresAt,
            user.CurrentBan.IsPermanent, user.CurrentBan.IsActive);

        var historyDtos = user.History.Select(h => new BanHistoryDto(
            h.Id, h.BannedByModeratorId.Value, h.Reason,
            h.BannedAt, h.ExpiresAt, h.UnbannedAt, h.UnbannedByModeratorId?.Value)).ToList();

        return new UserDto(
            user.Id.Value, user.Role.ToString(), user.Status.ToString(), user.CreatedAt,
            channelDto, currentBanDto, historyDtos);
    }
}