namespace MediaService.Domain.Constants;

public static class MediaBuckets
{
    public const string RawVideos = "raw-videos";
    public const string Variants = "variants";
    public const string Thumbnails = "thumbnails";
    public const string Images = "images";

    public static readonly HashSet<string> Public = [Variants, Thumbnails, Images];
}