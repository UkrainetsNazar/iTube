using AuthService.Application.Interfaces;
using MassTransit;
using Shared.Domain.Contracts.User;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace AuthService.Infrastructure.Consumers;

public sealed class UserBannedConsumer(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
) : IConsumer<UserBannedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserBannedIntegrationEvent> context)
    {
        var user = await userRepository.GetByIdAsync(
            UserId.From(context.Message.UserId),
            context.CancellationToken);
 
        if (user is null) return;
 
        user.ApplyBan();
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}