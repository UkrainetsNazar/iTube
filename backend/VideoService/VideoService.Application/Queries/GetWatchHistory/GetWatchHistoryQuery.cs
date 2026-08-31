using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;

namespace VideoService.Application.Queries.GetWatchHistory;

public sealed record GetWatchHistoryQuery(Guid UserId, int Page, int PageSize) : IRequest<Result<PagedVideosDto>>;