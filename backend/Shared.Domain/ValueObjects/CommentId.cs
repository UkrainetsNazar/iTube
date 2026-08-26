namespace Shared.Domain.ValueObjects;

public sealed record CommentId(Guid Value)
{
    public static CommentId New() => new(Guid.NewGuid());
}