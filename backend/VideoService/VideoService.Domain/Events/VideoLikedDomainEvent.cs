using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Enums;

namespace VideoService.Domain.Events;

public sealed record VideoLikedDomainEvent(VideoId VideoId, Guid UserId, ReactionType Type) : DomainEvent;