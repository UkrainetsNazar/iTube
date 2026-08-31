namespace UserService.Application.DTOs;

public sealed record UserDto(
    Guid Id,
    string Role,
    string Status,
    DateTime CreatedAt,
    ChannelSummaryDto? Channel,
    BanDto? CurrentBan,
    IReadOnlyList<BanHistoryDto> BanHistory);