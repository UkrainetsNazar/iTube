using MediaService.Application.Commands.ProcessVideo;
using MediaService.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaService.Infrastructure.Processing;

public sealed class MediaProcessingBackgroundService(
    IMediaProcessingQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<MediaProcessingBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var mediaAssetId in queue.DequeueAllAsync(stoppingToken))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                var result = await sender.Send(new ProcessVideoCommand(mediaAssetId), stoppingToken);

                if (result.IsFailure)
                    logger.LogWarning("Processing failed for {MediaAssetId}: {Error}", mediaAssetId.Value, result.Error);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error processing {MediaAssetId}", mediaAssetId.Value);
            }
        }
    }
}