using Shared.Domain.Common;

namespace VideoService.Domain.ValueObjects;

public sealed record VideoDescription
{
    public string Value { get; }
    private VideoDescription(string value) => Value = value;

    public static Result<VideoDescription> Create(string? value)
    {
        value ??= string.Empty;

        if (value.Length > 5000)
            return Result.Failure<VideoDescription>(Error.Validation("VideoDescription.TooLong", "Description cannot exceed 5000 characters."));

        return Result.Success(new VideoDescription(value.Trim()));
    }
}