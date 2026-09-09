using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Domain.Contracts.Media_Video;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Consumers;

public sealed class VideoPublishedConsumer(
    IChannelRepository channelRepository,
    IUnitOfWork unitOfWork,
    ILogger<VideoPublishedConsumer> logger) : IConsumer<VideoPublishedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<VideoPublishedIntegrationEvent> context)
    {
        var message = context.Message;

        var channel = await channelRepository.GetByIdAsync(ChannelId.From(message.AuthorId), context.CancellationToken);
        if (channel is null)
        {
            logger.LogWarning("VideoPublished for unknown channel {ChannelId}", message.AuthorId);
            return;
        }

        channel.IncrementVideoCount();
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}