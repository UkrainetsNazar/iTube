namespace MediaService.Application.DTO;

public sealed record MediaAssetStatusDto(Guid MediaAssetId, string Status, string? FailureReason);