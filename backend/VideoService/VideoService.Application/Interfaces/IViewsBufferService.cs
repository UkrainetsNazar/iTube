using Shared.Domain.ValueObjects;

namespace VideoService.Application.Interfaces;

public interface IViewsBufferService
{
    Task IncrementAsync(VideoId videoId, CancellationToken ct);
    Task<IReadOnlyDictionary<VideoId, long>> FlushAsync(CancellationToken ct);
}