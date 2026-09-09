using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Domain.Contracts.Media_Video;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;

namespace VideoService.Infrastructure.Consumers;

public sealed class MediaProcessingFailedIntegrationEventConsumer(
    IVideoRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<MediaProcessingFailedIntegrationEventConsumer> logger) : IConsumer<MediaProcessingFailedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<MediaProcessingFailedIntegrationEvent> context)
    {
        var message = context.Message;

        if (message.VideoId is null)
            return;

        var video = await repository.GetByIdAsync(new VideoId(message.VideoId.Value), context.CancellationToken);
        if (video is null)
        {
            logger.LogWarning("MediaProcessingFailed for unknown video {VideoId}", message.VideoId);
            return;
        }

        var result = video.MarkAsFailed(message.Reason);
        if (result.IsFailure)
        {
            logger.LogWarning("Could not mark video {VideoId} as failed: {Error}", message.VideoId, result.Error);
            return;
        }

        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}