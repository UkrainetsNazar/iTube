using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Contracts.Media_Video;
using VideoService.Domain.Entities;
using VideoService.Infrastructure.Persistence;

namespace VideoService.Infrastructure.Consumers;

public sealed class UserSubscribedIntegrationEventConsumer(VideoDbContext dbContext)
    : IConsumer<UserSubscribedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserSubscribedIntegrationEvent> context)
    {
        var message = context.Message;

        var exists = await dbContext.UserSubscriptions.AnyAsync(
            s => s.SubscriberId == message.SubscriberId && s.ChannelId == message.ChannelId,
            context.CancellationToken);

        if (exists) return;

        dbContext.UserSubscriptions.Add(new UserSubscription(message.SubscriberId, message.ChannelId, message.SubscribedAt));
        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}