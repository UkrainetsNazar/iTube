using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace VideoService.Domain.Events;

public sealed record VideoUploadedDomainEvent(VideoId VideoId, Guid AuthorId, string Title) : DomainEvent;