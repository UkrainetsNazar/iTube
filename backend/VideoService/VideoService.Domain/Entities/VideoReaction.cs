using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Enums;
using VideoService.Domain.Events;

namespace VideoService.Domain.Entities;

public sealed class VideoReaction : AggregateRoot<VideoReactionId>
{
    public VideoId VideoId { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public ReactionType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private VideoReaction() { }

    private VideoReaction(VideoReactionId id, VideoId videoId, Guid userId, ReactionType type) : base(id)
    {
        VideoId = videoId;
        UserId = userId;
        Type = type;
        CreatedAt = DateTime.UtcNow;
    }

    public static VideoReaction Create(VideoId videoId, Guid userId, ReactionType type)
    {
        var reaction = new VideoReaction(VideoReactionId.New(), videoId, userId, type);
        reaction.RaiseDomainEvent(new VideoLikedDomainEvent(videoId, userId, type));
        return reaction;
    }

    public void ChangeType(ReactionType newType) => Type = newType;
}