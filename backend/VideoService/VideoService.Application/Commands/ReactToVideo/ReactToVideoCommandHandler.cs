using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Domain.Enums;

namespace VideoService.Application.Commands.ReactToVideo;

public sealed class ReactToVideoCommandHandler(
    IVideoReactionRepository reactionRepository,
    IVideoRepository videoRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ReactToVideoCommand, Result>
{
    public async Task<Result> Handle(ReactToVideoCommand request, CancellationToken ct)
    {
        var video = await videoRepository.GetByIdAsync(request.VideoId, ct);
        if (video is null)
            return Result.Failure(Error.NotFound("Video.NotFound", "Video not found."));

        var existing = await reactionRepository.GetByVideoAndUserAsync(request.VideoId, request.UserId, ct);

        if (existing is null)
        {
            var reaction = VideoReaction.Create(request.VideoId, request.UserId, request.Type);
            reactionRepository.Add(reaction);

            video.ApplyReactionCounts(
                likesDelta: request.Type == ReactionType.Like ? 1 : 0,
                dislikesDelta: request.Type == ReactionType.Dislike ? 1 : 0);
        }
        else if (existing.Type == request.Type)
        {
            reactionRepository.Remove(existing);

            video.ApplyReactionCounts(
                likesDelta: request.Type == ReactionType.Like ? -1 : 0,
                dislikesDelta: request.Type == ReactionType.Dislike ? -1 : 0);
        }
        else
        {
            existing.ChangeType(request.Type);
            reactionRepository.Update(existing);

            video.ApplyReactionCounts(
                likesDelta: request.Type == ReactionType.Like ? 1 : -1,
                dislikesDelta: request.Type == ReactionType.Dislike ? 1 : -1);
        }

        videoRepository.Update(video);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}