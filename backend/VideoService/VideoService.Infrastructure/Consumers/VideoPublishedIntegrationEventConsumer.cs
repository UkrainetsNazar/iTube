using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Domain.Contracts.Media_Video;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;
using VideoService.Domain.Enums;

namespace VideoService.Infrastructure.Consumers;

public sealed class VideoPublishedIntegrationEventConsumer(
    IVideoRepository repository,
    IVideoSearchIndex searchIndex,
    ILogger<VideoPublishedIntegrationEventConsumer> logger) : IConsumer<VideoPublishedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<VideoPublishedIntegrationEvent> context)
    {
        var videoId = new VideoId(context.Message.VideoId);
        var video = await repository.GetByIdAsync(videoId, context.CancellationToken);

        if (video is null)
        {
            logger.LogWarning("VideoPublished event for unknown video {VideoId}", videoId.Value);
            return;
        }
        
        if (video.Status != VideoStatus.Published || video.Visibility != VideoVisibility.Public)
            return;

        var document = new VideoSearchDocument(
            video.Id.Value, video.Title.Value, video.Description.Value,
            video.Tags.Select(t => t.Value).ToList(), video.AuthorId,
            video.ThumbnailUrl, video.ViewsCount, video.PublishedAt ?? DateTime.UtcNow);

        await searchIndex.IndexVideoAsync(document, context.CancellationToken);
    }
}