using Shared.Domain.Common;
using Shared.Domain.Primitives;

namespace Shared.Domain.ValueObjects;

public sealed record MediaReference : ValueObject
{
    public string Bucket { get; }
    public string Key { get; }
    public string? Url { get; }

    private MediaReference(string bucket, string key, string? url)
    {
        Bucket = bucket;
        Key = key;
        Url = url;
    }

    public static Result<MediaReference> Create(string bucket, string key, string? url = null)
    {
        if (string.IsNullOrWhiteSpace(bucket))
            return Result.Failure<MediaReference>(Error.Validation("MediaReference.EmptyBucket", "Bucket cannot be empty."));

        if (string.IsNullOrWhiteSpace(key))
            return Result.Failure<MediaReference>(Error.Validation("MediaReference.EmptyKey", "Key cannot be empty."));

        return new MediaReference(bucket, key, url);
    }
}