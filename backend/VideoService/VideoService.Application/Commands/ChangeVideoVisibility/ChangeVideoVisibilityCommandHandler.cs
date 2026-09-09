using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using VideoService.Application.Interfaces;
using VideoService.Domain.Enums;

namespace VideoService.Application.Commands.ChangeVideoVisibility;

public sealed class ChangeVideoVisibilityCommandHandler(
    IVideoRepository repository,
    IUnitOfWork unitOfWork,
    IVideoSearchIndex searchIndex) : IRequestHandler<ChangeVideoVisibilityCommand, Result>
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

        await unitOfWork.SaveChangesAsync(ct);

        if (request.Visibility == VideoVisibility.Private)
        {
            await searchIndex.DeleteVideoAsync(video.Id, ct);
        }
        else
        {
            await searchIndex.IndexVideoAsync(new VideoSearchDocument(
                video.Id.Value, video.Title.Value, video.Description.Value,
                video.Tags.Select(t => t.Value).ToList(), video.AuthorId,
                video.ThumbnailUrl, video.ViewsCount, video.PublishedAt ?? DateTime.UtcNow), ct);
        }

        return Result.Success();
    }
}