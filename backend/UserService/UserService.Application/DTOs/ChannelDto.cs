namespace UserService.Application.DTOs;

public sealed record ChannelDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    string? Description,
    string? AvatarUrl,
    string? BannerUrl,
    int SubscribersCount,
    int VideoCount,
    string Status,
    DateTime CreatedAt);