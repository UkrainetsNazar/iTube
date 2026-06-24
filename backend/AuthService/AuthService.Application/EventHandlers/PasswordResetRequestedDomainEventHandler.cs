using AuthService.Application.Interfaces;
using AuthService.Domain.Events;
using MediatR;

namespace AuthService.Application.EventHandlers;

public sealed class PasswordResetRequestedDomainEventHandler(IEmailSender emailSender)
    : INotificationHandler<PasswordResetRequestedDomainEvent>
{
    public async Task Handle(PasswordResetRequestedDomainEvent notification, CancellationToken ct)
        => await emailSender.SendAsync(
            to: notification.Email,
            subject: "ITube — Password Reset",
            body: $"""
                   <h2>Password Reset Request</h2>
                   <p>Use the token below to reset your password. It expires in 1 hour.</p>
                   <p><strong>{notification.RawToken}</strong></p>
                   <p>If you did not request this, ignore this email.</p>
                   """,
            ct: ct);
}