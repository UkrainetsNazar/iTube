using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;

namespace VideoService.Application.Queries.GetRecommendations;

public sealed record GetRecommendationsQuery(int Count) : IRequest<Result<IReadOnlyList<VideoDto>>>;