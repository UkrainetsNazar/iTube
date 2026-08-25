using MediaService.Domain.Enums;
using MediaService.Domain.Events;
using Shared.Domain.Common;
using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace MediaService.Domain.Entities;

public sealed class MediaAsset : AggregateRoot<MediaAssetId>
{
    public string OriginalFileName { get; private set; } = null!;
    public MediaType MediaType { get; private set; }
    public MediaProcessingStatus Status { get; private set; }
    public string RawStoragePath { get; private set; } = null!;
    public string? ThumbnailStoragePath { get; private set; }
    public Guid UploadedBy { get; private set; }

    public Guid? VideoId { get; private set; }
    public Guid? OwningChannelId { get; private set; }

    public string? FailureReason { get; private set; }

    private readonly List<MediaVariant> _variants = [];
    public IReadOnlyList<MediaVariant> Variants => _variants.AsReadOnly();

    private MediaAsset()
    {
    }

    private MediaAsset(
        MediaAssetId id,
        string originalFileName,
        MediaType mediaType,
        string rawStoragePath,
        Guid uploadedBy,
        Guid? videoId,
        Guid? owningChannelId) : base(id)
    {
        OriginalFileName = originalFileName;
        MediaType = mediaType;
        RawStoragePath = rawStoragePath;
        UploadedBy = uploadedBy;
        VideoId = videoId;
        OwningChannelId = owningChannelId;
        Status = MediaProcessingStatus.Pending;
    }

    public static MediaAsset UploadRawVideo(
        string originalFileName, string rawStoragePath, Guid uploadedBy, Guid videoId)
    {
        var asset = new MediaAsset(
            MediaAssetId.New(), originalFileName, MediaType.RawVideo,
            rawStoragePath, uploadedBy, videoId, owningChannelId: null);

        asset.RaiseDomainEvent(new MediaUploadedDomainEvent(asset.Id, videoId));
        return asset;
    }

    public static MediaAsset UploadChannelImage(
        string originalFileName, string rawStoragePath, Guid uploadedBy,
        Guid channelId, MediaType mediaType)
    {
        if (mediaType is not (MediaType.Avatar or MediaType.Banner))
            throw new ArgumentException("Must be Avatar or Banner.", nameof(mediaType));

        var asset = new MediaAsset(
            MediaAssetId.New(), originalFileName, mediaType,
            rawStoragePath, uploadedBy, videoId: null, owningChannelId: channelId)
        {
            Status = MediaProcessingStatus.Completed
        };

        return asset;
    }

    public Result StartProcessing()
    {
        if (Status != MediaProcessingStatus.Pending)
            return Result.Failure(Error.Conflict(
                "MediaAsset.InvalidState", $"Cannot start processing from status {Status}."));

        Status = MediaProcessingStatus.Processing;
        return Result.Success();
    }

    public Result CompleteProcessing(string thumbnailStoragePath, IEnumerable<MediaVariant> variants)
    {
        if (Status != MediaProcessingStatus.Processing)
            return Result.Failure(Error.Conflict(
                "MediaAsset.InvalidState", $"Cannot complete processing from status {Status}."));

        ThumbnailStoragePath = thumbnailStoragePath;
        _variants.AddRange(variants);
        Status = MediaProcessingStatus.Completed;

        RaiseDomainEvent(new MediaProcessingCompletedDomainEvent(
            Id, VideoId!.Value, ThumbnailStoragePath, _variants.ToList()));

        return Result.Success();
    }

    public Result FailProcessing(string reason)
    {
        if (Status != MediaProcessingStatus.Processing)
            return Result.Failure(Error.Conflict(
                "MediaAsset.InvalidState", $"Cannot fail processing from status {Status}."));

        Status = MediaProcessingStatus.Failed;
        FailureReason = reason;

        RaiseDomainEvent(new MediaProcessingFailedDomainEvent(Id, VideoId, reason));
        return Result.Success();
    }
}