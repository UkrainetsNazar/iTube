using Shared.Domain.Common;
using Shared.Domain.Primitives;

namespace AuthService.Domain.ValueObjects;
public sealed record PasswordHash : ValueObject
{
    public string Value { get; }

    private PasswordHash(string value) => Value = value;

    public static Result<PasswordHash> Create(string? hashedValue)
    {
        if (string.IsNullOrWhiteSpace(hashedValue))
            return Result.Failure<PasswordHash>(Error.Validation("PasswordHash.Empty", "The password hash cannot be empty."));

        return new PasswordHash(hashedValue);
    }

    public override string ToString() => "******";
}