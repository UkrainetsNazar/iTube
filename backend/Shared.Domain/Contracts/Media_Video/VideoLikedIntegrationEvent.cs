namespace Shared.Domain.Contracts.Media_Video;

public sealed record VideoLikedIntegrationEvent(
    Guid VideoId,
    Guid UserId,
    bool IsLike);