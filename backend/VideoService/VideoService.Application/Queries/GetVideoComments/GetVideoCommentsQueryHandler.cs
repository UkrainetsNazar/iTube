using MediatR;
using Shared.Domain.Common;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Queries.GetVideoComments;

public sealed class GetVideoCommentsQueryHandler(ICommentRepository commentRepository)
    : IRequestHandler<GetVideoCommentsQuery, Result<PagedCommentsDto>>
{
    public async Task<Result<PagedCommentsDto>> Handle(GetVideoCommentsQuery request, CancellationToken ct)
    {
        var (comments, total) = await commentRepository.GetByVideoAsync(request.VideoId, request.Page, request.PageSize, ct);
        var items = comments.Select(c => new CommentDto(c.Id.Value, c.AuthorId, c.Text, c.CreatedAt)).ToList();
        return Result.Success(new PagedCommentsDto(items, total, request.Page, request.PageSize));
    }
}