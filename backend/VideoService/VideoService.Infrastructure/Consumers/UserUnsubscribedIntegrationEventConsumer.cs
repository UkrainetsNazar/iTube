using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Contracts.Media_Video;
using VideoService.Infrastructure.Persistence;

namespace VideoService.Infrastructure.Consumers;

public sealed class UserUnsubscribedIntegrationEventConsumer(VideoDbContext dbContext)
    : IConsumer<UserUnsubscribedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserUnsubscribedIntegrationEvent> context)
    {
        var message = context.Message;

        var subscription = await dbContext.UserSubscriptions.FirstOrDefaultAsync(
            s => s.SubscriberId == message.SubscriberId && s.ChannelId == message.ChannelId,
            context.CancellationToken);

        if (subscription is null) return;

        dbContext.UserSubscriptions.Remove(subscription);
        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}