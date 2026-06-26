using MassTransit;
using MediatR;
using Shared.Domain.Contracts.User;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers;

public sealed class RoleChangedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<RoleChanged>
{
    public async Task Handle(RoleChanged notification, CancellationToken ct)
        => await publishEndpoint.Publish(
            new RoleChangedIntegrationEvent(
                notification.UserId.Value,
                notification.NewRole.ToString(),
                notification.OccurredAt),
            ct);
}