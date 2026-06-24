using AuthService.Domain.Events;
using MassTransit;
using MediatR;
using Shared.Domain.Contracts.Auth;

namespace AuthService.Application.EventHandlers;

public sealed class EmailConfirmedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<EmailConfirmedDomainEvent>
{
    public async Task Handle(EmailConfirmedDomainEvent notification, CancellationToken ct)
        => await publishEndpoint.Publish(
            new EmailConfirmedIntegrationEvent(
                notification.UserId.Value,
                notification.Email,
                notification.OccurredAt),
            ct);
}