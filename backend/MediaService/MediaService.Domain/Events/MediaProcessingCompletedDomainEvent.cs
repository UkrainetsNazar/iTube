using MediaService.Domain.Entities;
using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace MediaService.Domain.Events;

public sealed record MediaProcessingCompletedDomainEvent(
    MediaAssetId MediaAssetId,
    Guid VideoId,
    string ThumbnailStoragePath,
    IReadOnlyCollection<MediaVariant> Variants) : DomainEvent;