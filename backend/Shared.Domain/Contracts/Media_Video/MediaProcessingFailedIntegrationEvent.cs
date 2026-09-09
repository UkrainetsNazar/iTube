namespace Shared.Domain.Contracts.Media_Video;

public sealed record MediaProcessingFailedIntegrationEvent(Guid MediaAssetId, Guid? VideoId, string Reason);