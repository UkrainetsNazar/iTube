using MediatR;
using Shared.Domain.Common;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.SearchVideos;

public sealed class SearchVideosQueryHandler(IVideoSearchIndex searchIndex)
    : IRequestHandler<SearchVideosQuery, Result<VideoSearchResult>>
{
    public async Task<Result<VideoSearchResult>> Handle(SearchVideosQuery request, CancellationToken ct)
    {
        if (request.Page < 1 || request.PageSize is < 1 or > 100)
            return Result.Failure<VideoSearchResult>(Error.Validation("Search.InvalidPaging", "page must be >= 1, pageSize between 1 and 100."));

        var result = await searchIndex.SearchAsync(request.Query, request.Tags, request.Page, request.PageSize, ct);
        return Result.Success(result);
    }
}