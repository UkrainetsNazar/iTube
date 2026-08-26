namespace Shared.Domain.ValueObjects;

public sealed record VideoReactionId(Guid Value)
{
    public static VideoReactionId New() => new(Guid.NewGuid());
}