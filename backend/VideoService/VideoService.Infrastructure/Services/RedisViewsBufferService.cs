using Shared.Domain.ValueObjects;
using StackExchange.Redis;
using VideoService.Application.Interfaces;

namespace VideoService.Infrastructure.Services;

public sealed class RedisViewsBufferService(IConnectionMultiplexer redis) : IViewsBufferService
{
    private const string KeyPrefix = "views:pending:";

    public Task IncrementAsync(VideoId videoId, CancellationToken ct)
    {
        var db = redis.GetDatabase();
        return db.StringIncrementAsync($"{KeyPrefix}{videoId.Value}");
    }

    public async Task<IReadOnlyDictionary<VideoId, long>> FlushAsync(CancellationToken ct)
    {
        var db = redis.GetDatabase();
        var server = redis.GetServer(redis.GetEndPoints()[0]);
        var result = new Dictionary<VideoId, long>();

        await foreach (var key in server.KeysAsync(pattern: $"{KeyPrefix}*"))
        {
            var value = await db.StringGetDeleteAsync(key);
            if (value.HasValue && long.TryParse(value, out var count) && count > 0
                && Guid.TryParse(key.ToString().Substring(KeyPrefix.Length), out var guid))
            {
                result[new VideoId(guid)] = count;
            }
        }

        return result;
    }
}