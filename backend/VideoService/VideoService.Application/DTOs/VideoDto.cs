using VideoService.Domain.Entities;

namespace VideoService.Application.DTOs;

public sealed record VideoDto(
    Guid Id, string Title, string Description, string Status, string? FailureReason, string Visibility,
    Guid AuthorId, string? ThumbnailUrl, long ViewsCount, int LikesCount, int DislikesCount,
    IReadOnlyList<string> Tags, IReadOnlyList<VideoSourceDto> Sources,
    DateTime CreatedAt, DateTime? PublishedAt)
{
    public static VideoDto FromEntity(Video video) => new(
        video.Id.Value, video.Title.Value, video.Description.Value,
        video.Status.ToString(), video.FailureReason, video.Visibility.ToString(),
        video.AuthorId, video.ThumbnailUrl, video.ViewsCount, video.LikesCount, video.DislikesCount,
        video.Tags.Select(t => t.Value).ToList(),
        video.Sources.Select(s => new VideoSourceDto(s.Resolution.ToString(), s.Url, s.Format)).ToList(),
        video.CreatedAt, video.PublishedAt);
}

public sealed record VideoSourceDto(string Resolution, string Url, string Format);