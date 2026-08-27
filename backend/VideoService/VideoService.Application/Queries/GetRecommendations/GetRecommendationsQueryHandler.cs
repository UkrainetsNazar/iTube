using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.GetRecommendations;

public sealed class GetRecommendationsQueryHandler(
    IVideoRepository videoRepository,
    IRecommendationFeedRepository feedRepository) : IRequestHandler<GetRecommendationsQuery, Result<IReadOnlyList<VideoDto>>>
{
    public async Task<Result<IReadOnlyList<VideoDto>>> Handle(GetRecommendationsQuery request, CancellationToken ct)
    {
        var results = new List<Domain.Entities.Video>();

        if (request.TargetVideoId is not null)
        {
            var target = await videoRepository.GetByIdAsync(request.TargetVideoId, ct);
            if (target is not null && target.Tags.Count > 0)
            {
                var tags = target.Tags.Select(t => t.Value).ToList();
                results.AddRange(await feedRepository.GetByTagOverlapAsync(tags, target.Id, request.Limit, ct));
            }
        }
        else if (request.UserId is not null)
        {
            results.AddRange(await feedRepository.GetByUserPreferenceAsync(request.UserId.Value, request.Limit, ct));
        }

        if (results.Count < request.Limit)
        {
            var excludeIds = results.Select(v => v.Id).ToList();
            if (request.TargetVideoId is not null)
                excludeIds.Add(request.TargetVideoId);

            var remaining = request.Limit - results.Count;
            var popular = await feedRepository.GetTopViewedAsync(remaining, excludeIds, ct);
            results.AddRange(popular);
        }

        return Result.Success<IReadOnlyList<VideoDto>>(results.Select(VideoDto.FromEntity).ToList());
    }
}