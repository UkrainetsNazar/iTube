using AuthService.Application.Interfaces;
using AuthService.Domain.Enums;
using MassTransit;
using Shared.Domain.Contracts.User;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
 
namespace AuthService.Infrastructure.Consumers;
 
public sealed class RoleChangedConsumer(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
) : IConsumer<RoleChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RoleChangedIntegrationEvent> context)
    {
        var user = await userRepository.GetByIdAsync(
            UserId.From(context.Message.UserId),
            context.CancellationToken);
 
        if (user is null) return;
 
        if (!Enum.TryParse<UserRole>(context.Message.NewRole, out var newRole)) return;
 
        user.ApplyRoleChanged(newRole);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}