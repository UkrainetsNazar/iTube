namespace UserService.Application.DTOs;

public sealed record ChannelSummaryDto(
    Guid Id, string Name, string? Description, int SubscribersCount, int VideoCount, string Status);