using MassTransit;
using MediaService.Domain.Events;
using MediatR;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Contracts.Media_Video;

namespace MediaService.Application.EventHandlers;

public sealed class MediaProcessingCompletedDomainEventHandler(
    IPublishEndpoint publishEndpoint, IConfiguration configuration)
    : INotificationHandler<MediaProcessingCompletedDomainEvent>
{
    public Task Handle(MediaProcessingCompletedDomainEvent notification, CancellationToken ct)
    {
        var baseUrl = configuration["Storage:PublicBaseUrl"]!.TrimEnd('/');

        var variants = notification.Variants
            .Select(v => new MediaVariantDto(v.Resolution.ToString(), $"{baseUrl}/{v.StoragePath}", "mp4", v.FileSizeBytes))
            .ToList();

        return publishEndpoint.Publish(new MediaProcessingCompletedIntegrationEvent(
            notification.MediaAssetId.Value,
            notification.VideoId,
            $"{baseUrl}/{notification.ThumbnailStoragePath}",
            variants), ct);
    }
}