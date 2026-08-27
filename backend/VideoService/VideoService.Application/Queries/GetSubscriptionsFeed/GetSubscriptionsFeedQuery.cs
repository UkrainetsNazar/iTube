using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;

namespace VideoService.Application.Queries.GetSubscriptionsFeed;

public sealed record GetSubscriptionsFeedQuery(Guid UserId, int Page, int PageSize) : IRequest<Result<IReadOnlyList<VideoDto>>>;