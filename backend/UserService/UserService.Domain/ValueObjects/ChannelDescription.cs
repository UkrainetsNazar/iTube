using Shared.Common;
using Shared.Domain.Primitives;

namespace UserService.Domain.ValueObjects;

public sealed record ChannelDescription : ValueObject
{
    public const int MaxLength = 1000;

    public string Value { get; }

    private ChannelDescription(string value) => Value = value;

    public static Result<ChannelDescription> Create(string? value)
    {
        var normalized = value ?? string.Empty;

        if (normalized.Length > MaxLength)
            return Result.Failure<ChannelDescription>(Error.Validation(
                "ChannelDescription.TooLong",
                $"The description can't exceed {MaxLength} characters."));

        return new ChannelDescription(normalized);
    }

    public override string ToString() => Value;
}