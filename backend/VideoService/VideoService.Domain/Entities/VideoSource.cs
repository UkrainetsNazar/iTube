using Shared.Domain.Enums;

namespace VideoService.Domain.Entities;

public sealed class VideoSource
{
    public Guid Id { get; private set; }
    public Resolution Resolution { get; private set; }
    public string Url { get; private set; } = null!;
    public string Format { get; private set; } = null!;

    private VideoSource() { }

    public VideoSource(Resolution resolution, string url, string format)
    {
        Id = Guid.NewGuid();
        Resolution = resolution;
        Url = url;
        Format = format;
    }
}