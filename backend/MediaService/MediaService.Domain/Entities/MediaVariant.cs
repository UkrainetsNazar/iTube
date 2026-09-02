using Shared.Domain.Enums;

namespace MediaService.Domain.Entities;

public sealed class MediaVariant
{
    public Guid Id { get; private set; }
    public Resolution Resolution { get; private set; }
    public int BitrateKbps { get; private set; }
    public string StoragePath { get; private set; } = null!;
    public long FileSizeBytes { get; private set; }

    private MediaVariant()
    {
    }

    public MediaVariant(Resolution resolution, int bitrateKbps, string storagePath, long fileSizeBytes)
    {
        Resolution = resolution;
        BitrateKbps = bitrateKbps;
        StoragePath = storagePath;
        FileSizeBytes = fileSizeBytes;
    }
}