using Shared.Domain.ValueObjects;

namespace MediaService.Application.Interfaces;

public interface IMediaProcessingQueue
{
    Task EnqueueAsync(MediaAssetId mediaAssetId, CancellationToken ct);
    IAsyncEnumerable<MediaAssetId> DequeueAllAsync(CancellationToken ct);
}