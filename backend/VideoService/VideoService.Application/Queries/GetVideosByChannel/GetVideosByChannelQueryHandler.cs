using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.GetVideosByChannel;

public sealed class GetVideosByChannelQueryHandler(IVideoRepository videoRepository)
    : IRequestHandler<GetVideosByChannelQuery, Result<IReadOnlyList<VideoDto>>>
{
    public async Task<Result<IReadOnlyList<VideoDto>>> Handle(GetVideosByChannelQuery request, CancellationToken ct)
    {
        var videos = await videoRepository.GetByChannelAsync(request.ChannelId, request.Page, request.PageSize, ct);
        return Result.Success<IReadOnlyList<VideoDto>>(videos.Select(VideoDto.FromEntity).ToList());
    }
}