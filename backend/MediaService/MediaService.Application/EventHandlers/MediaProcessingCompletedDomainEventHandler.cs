using MassTransit;
using MediaService.Domain.Events;
using MediatR;
using Shared.Domain.Contracts.Media_Video;

namespace MediaService.Application.EventHandlers;

public sealed class MediaProcessingCompletedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<MediaProcessingCompletedDomainEvent>
{
    public Task Handle(MediaProcessingCompletedDomainEvent notification, CancellationToken ct)
    {
        var variants = notification.Variants
            .Select(v => new MediaVariantDto(v.Resolution.ToString(), v.StoragePath, "mp4", v.FileSizeBytes))
            .ToList();

        return publishEndpoint.Publish(new MediaProcessingCompletedIntegrationEvent(
            notification.MediaAssetId.Value,
            notification.VideoId,
            notification.ThumbnailStoragePath,
            variants), ct);
    }
}