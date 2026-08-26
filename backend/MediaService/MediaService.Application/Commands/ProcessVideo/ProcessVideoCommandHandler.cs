using MediaService.Application.Interfaces;
using MediaService.Domain.Constants;
using MediaService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;

namespace MediaService.Application.Commands.ProcessVideo;

public sealed class ProcessVideoCommandHandler(
    IMediaAssetRepository repository,
    IUnitOfWork unitOfWork,
    IVideoStorageService storageService,
    IVideoProcessor videoProcessor,
    ILogger<ProcessVideoCommandHandler> logger) : IRequestHandler<ProcessVideoCommand, Result>
{
    public async Task<Result> Handle(ProcessVideoCommand request, CancellationToken ct)
    {
        var asset = await repository.GetByIdAsync(request.MediaAssetId, ct);
        if (asset is null)
            return Result.Failure(Error.NotFound("MediaAsset.NotFound", "Media asset not found."));

        var startResult = asset.StartProcessing();
        if (startResult.IsFailure)
            return startResult;

        await unitOfWork.SaveChangesAsync(ct);

        string? localInputPath = null;
        try
        {
            localInputPath = await storageService.DownloadToTempFileAsync(asset.RawStoragePath, ct);
            var processed = await videoProcessor.ProcessAsync(localInputPath, ct);

            await using var thumbStream = File.OpenRead(processed.ThumbnailLocalPath);
            var thumbnailStoragePath = await storageService.UploadAsync(
                thumbStream, MediaBuckets.Thumbnails, $"{asset.Id.Value}.jpg", "image/jpeg", ct);

            var variants = new List<MediaVariant>();
            foreach (var v in processed.Variants)
            {
                await using var variantStream = File.OpenRead(v.LocalFilePath);
                var variantPath = await storageService.UploadAsync(
                    variantStream, MediaBuckets.Variants, $"{asset.Id.Value}/{v.Resolution}.mp4", "video/mp4", ct);

                variants.Add(new MediaVariant(v.Resolution, v.BitrateKbps, variantPath, v.FileSizeBytes));
                File.Delete(v.LocalFilePath);
            }

            File.Delete(processed.ThumbnailLocalPath);

            var completeResult = asset.CompleteProcessing(thumbnailStoragePath, variants);
            if (completeResult.IsFailure)
                return completeResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Processing failed for media asset {MediaAssetId}", asset.Id.Value);
            asset.FailProcessing(ex.Message);
        }
        finally
        {
            if (localInputPath is not null && File.Exists(localInputPath))
                File.Delete(localInputPath);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}