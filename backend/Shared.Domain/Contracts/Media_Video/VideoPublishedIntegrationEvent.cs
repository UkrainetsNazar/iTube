namespace Shared.Domain.Contracts.Media_Video;

public sealed record VideoPublishedIntegrationEvent(
    Guid VideoId,
    Guid AuthorId,
    DateTime PublishedAtUtc);