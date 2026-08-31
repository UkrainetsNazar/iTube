namespace UserService.Application.DTOs;

public sealed record BanHistoryDto(
    Guid Id, Guid BannedByModeratorId, string Reason,
    DateTime BannedAt, DateTime? ExpiresAt, DateTime? UnbannedAt, Guid? UnbannedByModeratorId);