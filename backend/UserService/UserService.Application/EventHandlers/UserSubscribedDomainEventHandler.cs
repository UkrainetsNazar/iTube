using MassTransit;
using MediatR;
using Shared.Domain.Contracts.Media_Video;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers;

public sealed class UserSubscribedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<UserSubscribed>
{
    public Task Handle(UserSubscribed notification, CancellationToken ct)
        => publishEndpoint.Publish(new UserSubscribedIntegrationEvent(
            notification.SubscriberId.Value, notification.TargetChannelId.Value, DateTime.UtcNow), ct);
}