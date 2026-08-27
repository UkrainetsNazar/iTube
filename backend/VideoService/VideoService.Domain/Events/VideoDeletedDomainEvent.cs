using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace VideoService.Domain.Events;

public sealed record VideoDeletedDomainEvent(VideoId VideoId) : DomainEvent;