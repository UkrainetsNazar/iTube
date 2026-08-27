using Shared.Domain.Common;
using Shared.Domain.Enums;
using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Enums;
using VideoService.Domain.Events;
using VideoService.Domain.ValueObjects;

namespace VideoService.Domain.Entities;

public sealed class Video : AggregateRoot<VideoId>
{
    public VideoTitle Title { get; private set; } = null!;
    public VideoDescription Description { get; private set; } = null!;
    public VideoStatus Status { get; private set; }
    public VideoVisibility Visibility { get; private set; }
    public Guid AuthorId { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public long ViewsCount { get; private set; }
    public int LikesCount { get; private set; }
    public int DislikesCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }

    private readonly List<Tag> _tags = [];
    public IReadOnlyList<Tag> Tags => _tags.AsReadOnly();

    private readonly List<VideoSource> _sources = [];
    public IReadOnlyList<VideoSource> Sources => _sources.AsReadOnly();

    private Video() { }

    private Video(VideoId id, VideoTitle title, VideoDescription description, IEnumerable<Tag> tags, Guid authorId)
        : base(id)
    {
        Title = title;
        Description = description;
        _tags.AddRange(tags);
        AuthorId = authorId;
        Status = VideoStatus.Draft;
        Visibility = VideoVisibility.Private;
        CreatedAt = DateTime.UtcNow;
    }

    public static Video Upload(VideoTitle title, VideoDescription description, IEnumerable<Tag> tags, Guid authorId)
    {
        var video = new Video(VideoId.New(), title, description, tags, authorId);
        video.RaiseDomainEvent(new VideoUploadedDomainEvent(video.Id, authorId, title.Value));
        return video;
    }

    public Result Publish(VideoVisibility visibility)
    {
        if (Status == VideoStatus.Deleted)
            return Result.Failure(Error.Conflict("Video.Deleted", "Cannot publish a deleted video."));

        if (_sources.Count == 0)
            return Result.Failure(Error.Conflict("Video.NoSources", "Video has no processed sources yet — wait for media processing to complete."));

        Status = VideoStatus.Published;
        Visibility = visibility;
        PublishedAt = DateTime.UtcNow;

        RaiseDomainEvent(new VideoPublishedDomainEvent(Id, AuthorId));
        return Result.Success();
    }

    public Result Delete()
    {
        if (Status == VideoStatus.Deleted)
            return Result.Failure(Error.Conflict("Video.AlreadyDeleted", "Video is already deleted."));

        Status = VideoStatus.Deleted;
        RaiseDomainEvent(new VideoDeletedDomainEvent(Id));
        return Result.Success();
    }

    public Result AttachSource(Resolution resolution, string url, string format)
    {
        if (_sources.Any(s => s.Resolution == resolution))
            return Result.Failure(Error.Conflict("Video.SourceExists", $"Source for {resolution} already exists."));

        _sources.Add(new VideoSource(resolution, url, format));
        return Result.Success();
    }

    public void SetThumbnail(string thumbnailUrl) => ThumbnailUrl = thumbnailUrl;

    public void ApplyViewsIncrement(long count) => ViewsCount += count;

    public void ApplyReactionCounts(int likesDelta, int dislikesDelta)
    {
        LikesCount = Math.Max(0, LikesCount + likesDelta);
        DislikesCount = Math.Max(0, DislikesCount + dislikesDelta);
    }
}