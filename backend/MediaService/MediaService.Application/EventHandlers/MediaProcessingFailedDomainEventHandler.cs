using MassTransit;
using MediaService.Domain.Events;
using MediatR;
using Shared.Domain.Contracts.Media_Video;

namespace MediaService.Application.EventHandlers;

public sealed class MediaProcessingFailedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<MediaProcessingFailedDomainEvent>
{
    public Task Handle(MediaProcessingFailedDomainEvent notification, CancellationToken ct)
        => publishEndpoint.Publish(new MediaProcessingFailedIntegrationEvent(
            notification.MediaAssetId.Value, notification.VideoId, notification.Reason), ct);
}