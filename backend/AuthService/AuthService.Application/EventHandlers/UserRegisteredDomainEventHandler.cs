using AuthService.Application.Interfaces;
using AuthService.Domain.Events;
using MediatR;

namespace AuthService.Application.EventHandlers;

public sealed class UserRegisteredDomainEventHandler(IEmailSender emailSender)
    : INotificationHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken ct)
        => await emailSender.SendAsync(
            to: notification.Email,
            subject: "ITube — Confirm your email",
            body: $"""
                   <h2>Welcome to ITube!</h2>
                   <p>Use the token below to confirm your email. It expires in 7 days.</p>
                   <p><strong>{notification.ConfirmationToken}</strong></p>
                   """,
            ct: ct);
}