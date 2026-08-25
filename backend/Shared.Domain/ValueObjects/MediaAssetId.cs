namespace Shared.Domain.ValueObjects;

public sealed record MediaAssetId(Guid Value)
{
    public static MediaAssetId New() => new(Guid.NewGuid());
}