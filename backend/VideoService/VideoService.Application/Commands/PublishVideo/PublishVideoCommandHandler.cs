using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Commands.PublishVideo;

public sealed class PublishVideoCommandHandler(
    IVideoRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<PublishVideoCommand, Result>
{
    public async Task<Result> Handle(PublishVideoCommand request, CancellationToken ct)
    {
        var video = await repository.GetByIdAsync(request.VideoId, ct);
        if (video is null)
            return Result.Failure(Error.NotFound("Video.NotFound", "Video not found."));

        if (video.AuthorId != request.RequestedBy)
            return Result.Failure(Error.Conflict("Video.NotOwner", "Only the author can publish this video."));

        var result = video.Publish(request.Visibility);
        if (result.IsFailure) return result;

        repository.Update(video);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}