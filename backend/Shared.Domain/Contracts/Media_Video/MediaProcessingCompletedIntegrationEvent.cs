namespace Shared.Domain.Contracts.Media_Video;

public sealed record MediaProcessingCompletedIntegrationEvent(
    Guid MediaAssetId,
    Guid VideoId,
    string ThumbnailUrl,
    IReadOnlyCollection<MediaVariantDto> Variants);

public sealed record MediaVariantDto(
    string Resolution,
    string Url,
    string Format,
    long FileSizeBytes);