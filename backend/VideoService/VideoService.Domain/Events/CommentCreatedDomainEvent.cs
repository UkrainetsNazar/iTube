using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace VideoService.Domain.Events;

public sealed record CommentCreatedDomainEvent(CommentId CommentId, VideoId VideoId, Guid AuthorId, string Text) : DomainEvent;