namespace Shared.Domain.ValueObjects;

public sealed record VideoId(Guid Value)
{
    public static VideoId New() => new(Guid.NewGuid());
}