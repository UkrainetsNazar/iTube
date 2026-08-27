using MassTransit;
using MediatR;
using Shared.Domain.Contracts.Media_Video;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers;

public sealed class UserUnsubscribedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<UserUnsubscribed>
{
    public Task Handle(UserUnsubscribed notification, CancellationToken ct)
        => publishEndpoint.Publish(new UserUnsubscribedIntegrationEvent(
            notification.SubscriberId.Value, notification.TargetChannelId.Value), ct);
}