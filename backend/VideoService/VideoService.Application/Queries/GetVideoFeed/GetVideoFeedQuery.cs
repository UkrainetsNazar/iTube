using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;

namespace VideoService.Application.Queries.GetVideoFeed;

public sealed record GetVideoFeedQuery(int Page, int PageSize) : IRequest<Result<IReadOnlyList<VideoDto>>>;