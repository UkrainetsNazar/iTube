using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.Queries.GetChannel;

public sealed class GetChannelQueryHandler(IChannelRepository channelRepository)
    : IRequestHandler<GetChannelQuery, Result<ChannelDto>>
{
    public async Task<Result<ChannelDto>> Handle(GetChannelQuery request, CancellationToken ct)
    {
        var channel = await channelRepository.GetByIdAsync(ChannelId.From(request.ChannelId), ct);
 
        if (channel is null)
            return Result.Failure<ChannelDto>(Error.NotFound("GetChannel.NotFound", "Channel not found."));
 
        return new ChannelDto(
            channel.Id.Value,
            channel.OwnerId.Value,
            channel.Name.Value,
            channel.Description?.Value,
            channel.Avatar?.Url,
            channel.Banner?.Url,
            channel.SubscribersCount,
            channel.VideoCount,
            channel.Status.ToString(),
            channel.CreatedAt);
    }
}