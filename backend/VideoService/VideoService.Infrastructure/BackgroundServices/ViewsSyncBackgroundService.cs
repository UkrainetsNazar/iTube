using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Domain.Interfaces;
using VideoService.Application.Interfaces;

namespace VideoService.Infrastructure.BackgroundServices;

public sealed class ViewsSyncBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<ViewsSyncBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(FlushInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var viewsBuffer = scope.ServiceProvider.GetRequiredService<IViewsBufferService>();
                var videoRepository = scope.ServiceProvider.GetRequiredService<IVideoRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var buffered = await viewsBuffer.FlushAsync(stoppingToken);
                if (buffered.Count == 0) continue;

                foreach (var (videoId, count) in buffered)
                {
                    var video = await videoRepository.GetByIdAsync(videoId, stoppingToken);
                    if (video is null) continue;

                    video.ApplyViewsIncrement(count);
                    videoRepository.Update(video);
                }

                await unitOfWork.SaveChangesAsync(stoppingToken);
                logger.LogInformation("Flushed views for {Count} videos", buffered.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error flushing views buffer");
            }
        }
    }
}