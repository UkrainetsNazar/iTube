using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Domain.ValueObjects;

namespace Shared.Infrastructure.Converters;

public sealed class ChannelIdConverter() : ValueConverter<ChannelId, Guid>(
    id => id.Value,
    value => ChannelId.From(value));