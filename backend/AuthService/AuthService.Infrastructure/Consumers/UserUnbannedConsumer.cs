using AuthService.Application.Interfaces;
using MassTransit;
using Shared.Domain.Contracts.User;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace AuthService.Infrastructure.Consumers;

public sealed class UserUnbannedConsumer(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
) : IConsumer<UserUnbannedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserUnbannedIntegrationEvent> context)
    {
        var user = await userRepository.GetByIdAsync(
            UserId.From(context.Message.UserId),
            context.CancellationToken);
 
        if (user is null) return;
 
        user.ApplyUnban();
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}