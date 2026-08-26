using Shared.Domain.Common;

namespace VideoService.Domain.ValueObjects;

public sealed record VideoTitle
{
    public string Value { get; }
    private VideoTitle(string value) => Value = value;

    public static Result<VideoTitle> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<VideoTitle>(Error.Validation("VideoTitle.Empty", "Title cannot be empty."));

        if (value.Length > 200)
            return Result.Failure<VideoTitle>(Error.Validation("VideoTitle.TooLong", "Title cannot exceed 200 characters."));

        return Result.Success(new VideoTitle(value.Trim()));
    }
}