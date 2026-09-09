namespace Shared.Domain.Contracts.Media_Video;

public sealed record VideoDeletedIntegrationEvent(Guid VideoId, Guid AuthorId, bool WasPublished);