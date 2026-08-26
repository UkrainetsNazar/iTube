using Shared.Domain.ValueObjects;
using VideoService.Domain.Entities;

namespace VideoService.Application.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(CommentId id, CancellationToken ct);
    void Add(Comment comment);
    void Update(Comment comment);
}