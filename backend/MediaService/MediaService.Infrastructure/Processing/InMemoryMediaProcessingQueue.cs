using System.Threading.Channels;
using MediaService.Application.Interfaces;
using Shared.Domain.ValueObjects;

namespace MediaService.Infrastructure.Processing;

public sealed class InMemoryMediaProcessingQueue : IMediaProcessingQueue
{
    private readonly Channel<MediaAssetId> _channel = Channel.CreateUnbounded<MediaAssetId>();

    public Task EnqueueAsync(MediaAssetId mediaAssetId, CancellationToken ct)
        => _channel.Writer.WriteAsync(mediaAssetId, ct).AsTask();

    public IAsyncEnumerable<MediaAssetId> DequeueAllAsync(CancellationToken ct)
        => _channel.Reader.ReadAllAsync(ct);
}