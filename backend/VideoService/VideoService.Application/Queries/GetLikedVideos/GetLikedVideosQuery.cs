using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;

namespace VideoService.Application.Queries.GetLikedVideos;

public sealed record GetLikedVideosQuery(Guid UserId, int Page, int PageSize) : IRequest<Result<PagedVideosDto>>;