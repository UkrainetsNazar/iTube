using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;

namespace VideoService.Application.Commands.AddComment;

public sealed class AddCommentCommandHandler(
    ICommentRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<AddCommentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddCommentCommand request, CancellationToken ct)
    {
        var commentResult = Comment.Create(request.VideoId, request.AuthorId, request.Text);
        if (commentResult.IsFailure) return Result.Failure<Guid>(commentResult.Error);

        repository.Add(commentResult.Value);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(commentResult.Value.Id.Value);
    }
}