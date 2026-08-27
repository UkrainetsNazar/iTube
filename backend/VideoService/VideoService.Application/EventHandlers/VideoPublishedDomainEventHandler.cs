using MassTransit;
using MediatR;
using Shared.Domain.Contracts.Media_Video;
using VideoService.Domain.Events;

namespace VideoService.Application.EventHandlers;

public sealed class VideoPublishedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<VideoPublishedDomainEvent>
{
    public Task Handle(VideoPublishedDomainEvent notification, CancellationToken ct)
        => publishEndpoint.Publish(new VideoPublishedIntegrationEvent(
            notification.VideoId.Value, notification.AuthorId, DateTime.UtcNow), ct);
}