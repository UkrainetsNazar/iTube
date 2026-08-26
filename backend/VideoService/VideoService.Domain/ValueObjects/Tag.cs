using Shared.Domain.Common;

namespace VideoService.Domain.ValueObjects;

public sealed record Tag
{
    public string Value { get; }
    private Tag(string value) => Value = value;

    public static Result<Tag> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Tag>(Error.Validation("Tag.Empty", "Tag cannot be empty."));

        if (value.Length > 30)
            return Result.Failure<Tag>(Error.Validation("Tag.TooLong", "Tag cannot exceed 30 characters."));

        return Result.Success(new Tag(value.Trim().ToLowerInvariant()));
    }
}