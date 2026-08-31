using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;

namespace VideoService.Application.Queries.GetMyVideos;

public sealed record GetMyVideosQuery(Guid AuthorId, int Page, int PageSize) : IRequest<Result<PagedVideosDto>>;