using Shared.Common;
using Shared.Domain.Primitives;

namespace UserService.Domain.ValueObjects;

public sealed record ChannelName : ValueObject
{
    public const int MinLength = 3;
    public const int MaxLength = 30;

    public string Value { get; }

    private ChannelName(string value) => Value = value;

    public static Result<ChannelName> Create(string? value)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        if (trimmed.Length is < MinLength or > MaxLength)
            return Result.Failure<ChannelName>(Error.Validation(
                "ChannelName.InvalidLength",
                $"The channel name must contain between {MinLength} and {MaxLength} characters."));

        return new ChannelName(trimmed);
    }

    public override string ToString() => Value;
}