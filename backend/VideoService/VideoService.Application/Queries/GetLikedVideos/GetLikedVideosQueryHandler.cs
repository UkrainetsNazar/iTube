using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.GetLikedVideos;

public sealed class GetLikedVideosQueryHandler(
    IVideoReactionRepository reactionRepository, IVideoRepository videoRepository)
    : IRequestHandler<GetLikedVideosQuery, Result<PagedVideosDto>>
{
    public async Task<Result<PagedVideosDto>> Handle(GetLikedVideosQuery request, CancellationToken ct)
    {
        var (videoIds, total) = await reactionRepository.GetLikedVideoIdsByUserAsync(request.UserId, request.Page, request.PageSize, ct);
        if (videoIds.Count == 0)
            return Result.Success(new PagedVideosDto([], total, request.Page, request.PageSize));

        var videos = await videoRepository.GetByIdsAsync(videoIds, ct);
        var videosById = videos.ToDictionary(v => v.Id);

        var ordered = videoIds
            .Where(videosById.ContainsKey)
            .Select(id => VideoDto.FromEntity(videosById[id]))
            .ToList();

        return Result.Success(new PagedVideosDto(ordered, total, request.Page, request.PageSize));
    }
}