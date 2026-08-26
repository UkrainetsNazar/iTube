using MediaService.Application.Interfaces;
using MediaService.Domain.Entities;
using MediaService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;

namespace MediaService.Infrastructure.Repositories;

public sealed class MediaAssetRepository(MediaDbContext context) : IMediaAssetRepository
{
    public Task<MediaAsset?> GetByIdAsync(MediaAssetId id, CancellationToken ct)
        => context.MediaAssets.FirstOrDefaultAsync(x => x.Id == id, ct);

    public void Add(MediaAsset asset) => context.MediaAssets.Add(asset);
    public void Update(MediaAsset asset) => context.MediaAssets.Update(asset);
}