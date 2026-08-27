using MediatR;
using Shared.Domain.Common;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.SearchVideos;

public sealed record SearchVideosQuery(string? Query, IReadOnlyList<string>? Tags, int Page, int PageSize)
    : IRequest<Result<VideoSearchResult>>;