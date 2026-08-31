namespace VideoService.Application.DTOs;

public sealed record PagedVideosDto(IReadOnlyList<VideoDto> Items, int TotalCount, int Page, int PageSize);