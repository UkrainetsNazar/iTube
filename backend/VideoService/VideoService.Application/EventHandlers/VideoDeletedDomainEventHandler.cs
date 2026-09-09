using MassTransit;
using MediatR;
using Shared.Domain.Contracts.Media_Video;
using VideoService.Domain.Events;

namespace VideoService.Application.EventHandlers;

public sealed class VideoDeletedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<VideoDeletedDomainEvent>
{
    public Task Handle(VideoDeletedDomainEvent notification, CancellationToken ct)
        => publishEndpoint.Publish(new VideoDeletedIntegrationEvent(
            notification.VideoId.Value, notification.AuthorId, notification.WasPublished), ct);
}