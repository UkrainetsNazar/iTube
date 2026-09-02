// VideoService.Application/Commands/RecordView/RecordViewCommandHandler.cs
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;

namespace VideoService.Application.Commands.RecordView;

public sealed class RecordViewCommandHandler(
    IVideoRepository videoRepository,
    IViewsBufferService viewsBuffer,
    IWatchHistoryRepository watchHistoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RecordViewCommand, Result>
{
    private const int MinWatchSecondsToCount = 30;

    private static readonly TimeSpan ViewCooldown = TimeSpan.FromHours(4);

    public async Task<Result> Handle(RecordViewCommand request, CancellationToken ct)
    {
        var video = await videoRepository.GetByIdAsync(request.VideoId, ct);
        if (video is null)
            return Result.Failure(Error.NotFound("Video.NotFound", "Video not found."));

        if (request.WatchedSeconds < MinWatchSecondsToCount)
            return Result.Success();

        if (request.UserId is null)
        {
            await viewsBuffer.IncrementAsync(request.VideoId, ct);
            return Result.Success();
        }

        var existing = await watchHistoryRepository.FindAsync(request.UserId.Value, request.VideoId, ct);

        if (existing is not null && DateTime.UtcNow - existing.WatchedAt < ViewCooldown)
        {
            existing.Touch();
            watchHistoryRepository.Update(existing);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }

        await viewsBuffer.IncrementAsync(request.VideoId, ct);

        if (existing is null)
            watchHistoryRepository.Add(new WatchHistoryEntry(request.UserId.Value, request.VideoId));
        else
        {
            existing.Touch();
            watchHistoryRepository.Update(existing);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}