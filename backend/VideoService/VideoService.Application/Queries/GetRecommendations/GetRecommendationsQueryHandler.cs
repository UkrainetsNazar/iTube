using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.GetRecommendations;

public sealed class GetRecommendationsQueryHandler(IRecommendationFeedRepository feedRepository)
    : IRequestHandler<GetRecommendationsQuery, Result<IReadOnlyList<VideoDto>>>
{
    public async Task<Result<IReadOnlyList<VideoDto>>> Handle(GetRecommendationsQuery request, CancellationToken ct)
    {
        var videos = await feedRepository.GetTopViewedAsync(request.Count, ct);
        return Result.Success<IReadOnlyList<VideoDto>>(videos.Select(VideoDto.FromEntity).ToList());
    }
}