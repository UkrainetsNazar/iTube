namespace Shared.Domain.ValueObjects;

public readonly record struct ChannelId(Guid Value)
{
    public static ChannelId New() => new(Guid.NewGuid());

    public static ChannelId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}