namespace UserService.Application.DTOs;

public sealed record BanDto(
    Guid Id, Guid BannedByModeratorId, string Reason,
    DateTime BannedAt, DateTime? ExpiresAt, bool IsPermanent, bool IsActive);