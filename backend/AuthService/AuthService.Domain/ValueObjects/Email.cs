using System.Text.RegularExpressions;
using Shared.Domain.Common;
using Shared.Domain.Primitives;

namespace AuthService.Domain.ValueObjects;

public sealed partial record Email : ValueObject
{
    public string Value { get; }
 
    private Email(string value) => Value = value;
 
    public static Result<Email> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Email>(Error.Validation("Email.Empty", "Email field cannot be left blank."));
 
        var normalized = value.Trim().ToLowerInvariant();
 
        if (normalized.Length > 256 || !EmailRegex().IsMatch(normalized))
            return Result.Failure<Email>(Error.Validation("Email.Invalid", "Invalid email format."));
 
        return new Email(normalized);
    }
 
    public override string ToString() => Value;
 
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}