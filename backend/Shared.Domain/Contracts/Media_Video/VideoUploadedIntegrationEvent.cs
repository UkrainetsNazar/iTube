namespace Shared.Domain.Contracts.Media_Video;

public sealed record VideoUploadedIntegrationEvent(
    Guid VideoId,
    Guid AuthorId,
    string Title,
    DateTime UploadedAtUtc);