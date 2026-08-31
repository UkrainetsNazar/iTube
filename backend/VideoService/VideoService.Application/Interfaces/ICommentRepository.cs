using Shared.Domain.ValueObjects;
using VideoService.Domain.Entities;

namespace VideoService.Application.Interfaces;

public interface ICommentRepository
{
    Task<(IReadOnlyList<Comment> Items, int TotalCount)> GetByVideoAsync(VideoId videoId, int page, int pageSize, CancellationToken ct);
    Task<Comment?> GetByIdAsync(CommentId id, CancellationToken ct);
    void Add(Comment comment);
    void Update(Comment comment);
}