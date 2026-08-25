using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace MediaService.Domain.Events;

public sealed record MediaUploadedDomainEvent(MediaAssetId MediaAssetId, Guid VideoId) : DomainEvent;