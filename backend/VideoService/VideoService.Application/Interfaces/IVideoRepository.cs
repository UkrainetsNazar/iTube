using Shared.Domain.ValueObjects;
using VideoService.Domain.Entities;

namespace VideoService.Application.Interfaces;

public interface IVideoRepository
{
    Task<Video?> GetByIdAsync(VideoId id, CancellationToken ct);
    void Add(Video video);
    void Update(Video video);
}