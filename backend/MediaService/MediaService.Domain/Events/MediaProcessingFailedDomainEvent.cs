using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace MediaService.Domain.Events;

public sealed record MediaProcessingFailedDomainEvent(
    MediaAssetId MediaAssetId,
    Guid? VideoId,
    string Reason) : DomainEvent;