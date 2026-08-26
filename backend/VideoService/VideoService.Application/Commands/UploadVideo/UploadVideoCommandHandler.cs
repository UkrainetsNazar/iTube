using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Domain.ValueObjects;

namespace VideoService.Application.Commands.UploadVideo;

public sealed class UploadVideoCommandHandler(
    IVideoRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<UploadVideoCommand, Result<UploadVideoResponse>>
{
    public async Task<Result<UploadVideoResponse>> Handle(UploadVideoCommand request, CancellationToken ct)
    {
        var titleResult = VideoTitle.Create(request.Title);
        if (titleResult.IsFailure) return Result.Failure<UploadVideoResponse>(titleResult.Error);

        var descriptionResult = VideoDescription.Create(request.Description);
        if (descriptionResult.IsFailure) return Result.Failure<UploadVideoResponse>(descriptionResult.Error);

        var tags = new List<Tag>();
        foreach (var raw in request.Tags ?? [])
        {
            var tagResult = Tag.Create(raw);
            if (tagResult.IsFailure) return Result.Failure<UploadVideoResponse>(tagResult.Error);
            tags.Add(tagResult.Value);
        }

        var video = Video.Upload(titleResult.Value, descriptionResult.Value, tags, request.AuthorId);

        repository.Add(video);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new UploadVideoResponse(video.Id.Value));
    }
}