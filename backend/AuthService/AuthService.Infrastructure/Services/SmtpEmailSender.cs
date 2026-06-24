using AuthService.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace AuthService.Infrastructure.Services;

public sealed class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(configuration["Smtp:From"]!));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            configuration["Smtp:Host"]!,
            int.Parse(configuration["Smtp:Port"]!),
            SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(
            configuration["Smtp:User"]!,
            configuration["Smtp:Pass"]!, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}