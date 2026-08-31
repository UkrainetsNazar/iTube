using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.GetMyVideos;

public sealed class GetMyVideosQueryHandler(IVideoRepository videoRepository)
    : IRequestHandler<GetMyVideosQuery, Result<PagedVideosDto>>
{
    public async Task<Result<PagedVideosDto>> Handle(GetMyVideosQuery request, CancellationToken ct)
    {
        var (videos, total) = await videoRepository.GetAllByAuthorAsync(request.AuthorId, request.Page, request.PageSize, ct);
        return Result.Success(new PagedVideosDto(videos.Select(VideoDto.FromEntity).ToList(), total, request.Page, request.PageSize));
    }
}