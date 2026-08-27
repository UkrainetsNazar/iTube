using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.GetSubscriptionsFeed;

public sealed class GetSubscriptionsFeedQueryHandler(
    IUserSubscriptionRepository subscriptionRepository,
    IVideoRepository videoRepository) : IRequestHandler<GetSubscriptionsFeedQuery, Result<IReadOnlyList<VideoDto>>>
{
    public async Task<Result<IReadOnlyList<VideoDto>>> Handle(GetSubscriptionsFeedQuery request, CancellationToken ct)
    {
        var channelIds = await subscriptionRepository.GetSubscribedChannelIdsAsync(request.UserId, ct);
        if (channelIds.Count == 0)
            return Result.Success<IReadOnlyList<VideoDto>>([]);

        var videos = await videoRepository.GetByChannelsAsync(channelIds, request.Page, request.PageSize, ct);
        return Result.Success<IReadOnlyList<VideoDto>>(videos.Select(VideoDto.FromEntity).ToList());
    }
}