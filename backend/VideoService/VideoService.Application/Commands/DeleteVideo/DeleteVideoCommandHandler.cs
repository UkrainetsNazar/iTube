// VideoService.Application/Commands/DeleteVideo/DeleteVideoCommandHandler.cs
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using VideoService.Application.Interfaces;

namespace VideoService.Application.Commands.DeleteVideo;

public sealed class DeleteVideoCommandHandler(
    IVideoRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteVideoCommand, Result>
{
    public async Task<Result> Handle(DeleteVideoCommand request, CancellationToken ct)
    {
        var video = await repository.GetByIdAsync(request.VideoId, ct);
        if (video is null)
            return Result.Failure(Error.NotFound("Video.NotFound", "Video not found."));

        if (video.AuthorId != request.RequestedBy)
            return Result.Failure(Error.Conflict("Video.NotOwner", "Only the author can delete this video."));

        var result = video.Delete();
        if (result.IsFailure) return result;

        repository.Remove(video);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}