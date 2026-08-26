using Shared.Domain.Common;
using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Events;

namespace VideoService.Domain.Entities;

public sealed class Comment : AggregateRoot<CommentId>
{
    public VideoId VideoId { get; private set; } = null!;
    public Guid AuthorId { get; private set; }
    public string Text { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    private Comment() { }

    private Comment(CommentId id, VideoId videoId, Guid authorId, string text) : base(id)
    {
        VideoId = videoId;
        AuthorId = authorId;
        Text = text;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Comment> Create(VideoId videoId, Guid authorId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result.Failure<Comment>(Error.Validation("Comment.Empty", "Comment text cannot be empty."));

        if (text.Length > 2000)
            return Result.Failure<Comment>(Error.Validation("Comment.TooLong", "Comment cannot exceed 2000 characters."));

        var comment = new Comment(CommentId.New(), videoId, authorId, text.Trim());
        comment.RaiseDomainEvent(new CommentCreatedDomainEvent(comment.Id, videoId, authorId, comment.Text));
        return Result.Success(comment);
    }

    public Result Delete()
    {
        if (IsDeleted)
            return Result.Failure(Error.Conflict("Comment.AlreadyDeleted", "Comment is already deleted."));

        IsDeleted = true;
        return Result.Success();
    }
}