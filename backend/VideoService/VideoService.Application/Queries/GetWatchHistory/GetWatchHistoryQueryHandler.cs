using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.GetWatchHistory;

public sealed class GetWatchHistoryQueryHandler(
    IWatchHistoryRepository watchHistoryRepository, IVideoRepository videoRepository)
    : IRequestHandler<GetWatchHistoryQuery, Result<PagedVideosDto>>
{
    public async Task<Result<PagedVideosDto>> Handle(GetWatchHistoryQuery request, CancellationToken ct)
    {
        var (entries, total) = await watchHistoryRepository.GetByUserAsync(request.UserId, request.Page, request.PageSize, ct);
        if (entries.Count == 0)
            return Result.Success(new PagedVideosDto([], total, request.Page, request.PageSize));

        var videoIds = entries.Select(e => e.VideoId).ToList();
        var videos = await videoRepository.GetByIdsAsync(videoIds, ct);
        var videosById = videos.ToDictionary(v => v.Id);

        // Preserve watch-order (most recent first, per repository's ordering) — not DB join order.
        var ordered = entries
            .Where(e => videosById.ContainsKey(e.VideoId))
            .Select(e => VideoDto.FromEntity(videosById[e.VideoId]))
            .ToList();

        return Result.Success(new PagedVideosDto(ordered, total, request.Page, request.PageSize));
    }
}