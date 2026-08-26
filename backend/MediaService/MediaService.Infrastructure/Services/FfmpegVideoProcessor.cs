using System.Diagnostics;
using MediaService.Application.Interfaces;
using MediaService.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace MediaService.Infrastructure.Services;

public sealed class FfmpegVideoProcessor(ILogger<FfmpegVideoProcessor> logger) : IVideoProcessor
{
    private static readonly (Resolution Resolution, string Size, int BitrateKbps)[] Targets =
    [
        (Resolution.R1080p, "1920x1080", 5000),
        (Resolution.R720p, "1280x720", 2800),
        (Resolution.R480p, "854x480", 1400),
    ];

    public async Task<VideoProcessingResult> ProcessAsync(string localInputFilePath, CancellationToken ct)
    {
        var thumbnailPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
        await RunFfmpegAsync($"-i \"{localInputFilePath}\" -ss 00:00:01.000 -vframes 1 \"{thumbnailPath}\"", ct);

        var variants = new List<ProcessedVariant>();
        foreach (var target in Targets)
        {
            var outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-{target.Resolution}.mp4");

            await RunFfmpegAsync(
                $"-i \"{localInputFilePath}\" -vf scale={target.Size} -b:v {target.BitrateKbps}k " +
                $"-c:v libx264 -preset fast -c:a aac -b:a 128k \"{outputPath}\"", ct);

            variants.Add(new ProcessedVariant(target.Resolution, target.BitrateKbps, outputPath, new FileInfo(outputPath).Length));
        }

        return new VideoProcessingResult(thumbnailPath, variants);
    }

    private async Task RunFfmpegAsync(string arguments, CancellationToken ct)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-y {arguments}",
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start ffmpeg.");
        var stderr = await process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);

        if (process.ExitCode != 0)
        {
            logger.LogError("ffmpeg failed (exit {ExitCode}): {Stderr}", process.ExitCode, stderr);
            throw new InvalidOperationException($"ffmpeg exited with code {process.ExitCode}: {stderr}");
        }
    }
}