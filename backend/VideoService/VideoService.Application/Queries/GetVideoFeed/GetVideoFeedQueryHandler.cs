using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.GetVideoFeed;

public sealed class GetVideoFeedQueryHandler(IRecommendationFeedRepository feedRepository)
    : IRequestHandler<GetVideoFeedQuery, Result<IReadOnlyList<VideoDto>>>
{
    public async Task<Result<IReadOnlyList<VideoDto>>> Handle(GetVideoFeedQuery request, CancellationToken ct)
    {
        var videos = await feedRepository.GetFeedAsync(request.Page, request.PageSize, ct);
        return Result.Success<IReadOnlyList<VideoDto>>(videos.Select(VideoDto.FromEntity).ToList());
    }
}