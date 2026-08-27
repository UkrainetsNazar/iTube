using MassTransit;
using Shared.Domain.Contracts.Media_Video;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;

namespace VideoService.Infrastructure.Consumers;

public sealed class VideoDeletedIntegrationEventConsumer(IVideoSearchIndex searchIndex)
    : IConsumer<VideoDeletedIntegrationEvent>
{
    public Task Consume(ConsumeContext<VideoDeletedIntegrationEvent> context)
        => searchIndex.DeleteVideoAsync(new VideoId(context.Message.VideoId), context.CancellationToken);
}