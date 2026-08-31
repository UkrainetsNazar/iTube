using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;

namespace VideoService.Application.Queries.GetVideoComments;

public sealed record GetVideoCommentsQuery(VideoId VideoId, int Page, int PageSize) : IRequest<Result<PagedCommentsDto>>;

public sealed record CommentDto(Guid Id, Guid AuthorId, string Text, DateTime CreatedAt);
public sealed record PagedCommentsDto(IReadOnlyList<CommentDto> Items, int TotalCount, int Page, int PageSize);