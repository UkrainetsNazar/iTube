using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace VideoService.Domain.Events;

public sealed record VideoPublishedDomainEvent(VideoId VideoId, Guid AuthorId) : DomainEvent;