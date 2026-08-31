using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Commands.ChangeVideoVisibility;

public sealed class ChangeVideoVisibilityCommandHandler(
    IVideoRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<ChangeVideoVisibilityCommand, Result>
{
    public async Task<Result> Handle(ChangeVideoVisibilityCommand request, CancellationToken ct)
    {
        var video = await repository.GetByIdAsync(request.VideoId, ct);
        if (video is null)
            return Result.Failure(Error.NotFound("Video.NotFound", "Video not found."));

        if (video.AuthorId != request.RequestedBy)
            return Result.Failure(Error.Conflict("Video.NotOwner", "Only the author can change this video's visibility."));

        var result = video.ChangeVisibility(request.Visibility);
        if (result.IsFailure) return result;

        repository.Update(video);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}