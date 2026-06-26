using MassTransit;
using MediatR;
using Shared.Domain.Contracts.Auth;
using UserService.Application.Commands.CreateUserAccount;

namespace UserService.Infrastructure.Consumers;

public sealed class EmailConfirmedConsumer(ISender sender)
    : IConsumer<EmailConfirmedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<EmailConfirmedIntegrationEvent> context)
    {
        var message = context.Message;
 
        await sender.Send(new CreateUserAccountCommand(
            message.UserId,
            message.Email));
    }
}