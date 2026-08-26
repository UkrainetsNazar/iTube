using MediaService.Domain.Enums;

namespace MediaService.Application.Interfaces;

public interface IVideoProcessor
{
    Task<VideoProcessingResult> ProcessAsync(string localInputFilePath, CancellationToken ct);
}

public sealed record VideoProcessingResult(
    string ThumbnailLocalPath,
    IReadOnlyCollection<ProcessedVariant> Variants);

public sealed record ProcessedVariant(
    Resolution Resolution,
    int BitrateKbps,
    string LocalFilePath,
    long FileSizeBytes);