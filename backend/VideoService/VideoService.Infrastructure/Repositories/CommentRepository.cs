using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Infrastructure.Persistence;

namespace VideoService.Infrastructure.Repositories;

public sealed class CommentRepository(VideoDbContext context) : ICommentRepository
{
    public Task<Comment?> GetByIdAsync(CommentId id, CancellationToken ct)
        => context.Comments.FirstOrDefaultAsync(c => c.Id == id, ct);

    public void Add(Comment comment) => context.Comments.Add(comment);
    public void Update(Comment comment) => context.Comments.Update(comment);
}