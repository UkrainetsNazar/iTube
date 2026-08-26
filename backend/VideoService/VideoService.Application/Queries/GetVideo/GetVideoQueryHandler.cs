using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;
using VideoService.Domain.Enums;

namespace VideoService.Application.Queries.GetVideo;

public sealed class GetVideoQueryHandler(
    IVideoRepository repository, IViewsBufferService viewsBuffer) : IRequestHandler<GetVideoQuery, Result<VideoDto>>
{
    public async Task<Result<VideoDto>> Handle(GetVideoQuery request, CancellationToken ct)
    {
        var video = await repository.GetByIdAsync(request.VideoId, ct);
        if (video is null || video.Status == VideoStatus.Deleted)
            return Result.Failure<VideoDto>(Error.NotFound("Video.NotFound", "Video not found."));

        if (video.Status == VideoStatus.Published)
            await viewsBuffer.IncrementAsync(video.Id, ct);

        return Result.Success(VideoDto.FromEntity(video));
    }
}