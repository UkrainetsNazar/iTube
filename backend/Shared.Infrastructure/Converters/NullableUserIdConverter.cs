using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Domain.ValueObjects;

namespace Shared.Infrastructure.Converters;

public sealed class NullableUserIdConverter() : ValueConverter<UserId?, Guid?>(
    id => id == null ? null : id.Value.Value,
    value => value == null ? null : UserId.From(value.Value));