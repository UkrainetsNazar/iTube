using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Domain.ValueObjects;

namespace Shared.Infrastructure.Converters;

public sealed class UserIdConverter() : ValueConverter<UserId, Guid>(
    id => id.Value,
    value => UserId.From(value));