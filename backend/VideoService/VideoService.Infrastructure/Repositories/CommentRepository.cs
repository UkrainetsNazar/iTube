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

    public async Task<(IReadOnlyList<Comment>, int)> GetByVideoAsync(VideoId videoId, int page, int pageSize, CancellationToken ct)
    {
        var query = context.Comments.Where(c => c.VideoId == videoId && !c.IsDeleted);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return (items, total);
    }
}