namespace Shared.Domain.Contracts.Media_Video;

public sealed record CommentCreatedIntegrationEvent(
    Guid CommentId,
    Guid VideoId,
    Guid AuthorId,
    string Text,
    DateTime CreatedAtUtc);