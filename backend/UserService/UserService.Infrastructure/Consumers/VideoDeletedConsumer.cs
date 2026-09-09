using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Domain.Contracts.Media_Video;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Consumers;

public sealed class VideoDeletedConsumer(
    IChannelRepository channelRepository,
    IUnitOfWork unitOfWork,
    ILogger<VideoDeletedConsumer> logger) : IConsumer<VideoDeletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<VideoDeletedIntegrationEvent> context)
    {
        var message = context.Message;

        if (!message.WasPublished)
            return;

        var channel = await channelRepository.GetByIdAsync(ChannelId.From(message.AuthorId), context.CancellationToken);
        if (channel is null)
        {
            logger.LogWarning("VideoDeleted for unknown channel {ChannelId}", message.AuthorId);
            return;
        }

        channel.DecrementVideoCount();
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}