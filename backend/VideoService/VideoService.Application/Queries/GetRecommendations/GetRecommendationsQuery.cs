using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using VideoService.Application.DTOs;

namespace VideoService.Application.Queries.GetRecommendations;

public sealed record GetRecommendationsQuery(VideoId? TargetVideoId, Guid? UserId, int Limit)
    : IRequest<Result<IReadOnlyList<VideoDto>>>;