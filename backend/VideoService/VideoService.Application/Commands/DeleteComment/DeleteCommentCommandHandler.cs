using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Commands.DeleteComment;

public sealed class DeleteCommentCommandHandler(
    ICommentRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteCommentCommand, Result>
{
    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken ct)
    {
        var comment = await repository.GetByIdAsync(request.CommentId, ct);
        if (comment is null)
            return Result.Failure(Error.NotFound("Comment.NotFound", "Comment not found."));

        if (comment.AuthorId != request.RequestedBy)
            return Result.Failure(Error.Conflict("Comment.NotOwner", "Only the author can delete this comment."));

        var result = comment.Delete();
        if (result.IsFailure) return result;

        repository.Update(comment);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}