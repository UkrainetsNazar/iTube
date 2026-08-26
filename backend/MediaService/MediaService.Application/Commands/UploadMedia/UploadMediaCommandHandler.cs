using MediaService.Application.Interfaces;
using MediaService.Domain.Constants;
using MediaService.Domain.Entities;
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;

namespace MediaService.Application.Commands.UploadMedia;

public sealed class UploadMediaCommandHandler(
    IMediaAssetRepository repository,
    IUnitOfWork unitOfWork,
    IVideoStorageService storageService,
    IMediaProcessingQueue processingQueue) : IRequestHandler<UploadMediaCommand, Result<UploadMediaResponse>>
{
    public async Task<Result<UploadMediaResponse>> Handle(UploadMediaCommand request, CancellationToken ct)
    {
        if (request.MediaType == Domain.Enums.MediaType.RawVideo && request.VideoId is null)
            return Result.Failure<UploadMediaResponse>(
                Error.Validation("Media.MissingVideoId", "videoId is required for raw video uploads."));

        if (request.MediaType is Domain.Enums.MediaType.Avatar or Domain.Enums.MediaType.Banner
            && request.ChannelId is null)
            return Result.Failure<UploadMediaResponse>(
                Error.Validation("Media.MissingChannelId", "channelId is required for avatar/banner uploads."));

        var objectKey = $"{Guid.NewGuid()}-{request.OriginalFileName}";
        
        var bucket = request.MediaType == Domain.Enums.MediaType.RawVideo
            ? MediaBuckets.RawVideos : MediaBuckets.Images;
        var storagePath = await storageService.UploadAsync(request.Content, bucket, objectKey, request.ContentType, ct);

        var asset = request.MediaType == Domain.Enums.MediaType.RawVideo
            ? MediaAsset.UploadRawVideo(request.OriginalFileName, storagePath, request.UploadedBy, request.VideoId!.Value)
            : MediaAsset.UploadChannelImage(request.OriginalFileName, storagePath, request.UploadedBy,
                request.ChannelId!.Value, request.MediaType);

        repository.Add(asset);
        await unitOfWork.SaveChangesAsync(ct);

        if (asset.MediaType == Domain.Enums.MediaType.RawVideo)
            await processingQueue.EnqueueAsync(asset.Id, ct);

        return Result.Success(new UploadMediaResponse(asset.Id.Value, asset.Status.ToString()));
    }
}