using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Domain.Contracts.Media_Video;
using Shared.Domain.Enums;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;

namespace VideoService.Infrastructure.Consumers;

public sealed class MediaProcessingCompletedConsumer(
    IVideoRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<MediaProcessingCompletedConsumer> logger) : IConsumer<MediaProcessingCompletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<MediaProcessingCompletedIntegrationEvent> context)
    {
        var message = context.Message;
        var video = await repository.GetByIdAsync(new VideoId(message.VideoId), context.CancellationToken);
        if (video is null)
        {
            logger.LogWarning("Received MediaProcessingCompleted for unknown video {VideoId}", message.VideoId);
            return;
        }

        video.SetThumbnail(message.ThumbnailUrl);

        foreach (var variant in message.Variants)
        {
            if (!Enum.TryParse<Resolution>(variant.Resolution, out var resolution))
            {
                logger.LogWarning("Unknown resolution '{Resolution}' for video {VideoId}", variant.Resolution, message.VideoId);
                continue;
            }

            video.AttachSource(resolution, variant.Url, variant.Format);
        }

        repository.Update(video);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}