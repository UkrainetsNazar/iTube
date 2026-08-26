using MediaService.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace MediaService.Application.Interfaces;

public interface IMediaAssetRepository
{
    Task<MediaAsset?> GetByIdAsync(MediaAssetId id, CancellationToken ct);
    void Add(MediaAsset asset);
    void Update(MediaAsset asset);
}