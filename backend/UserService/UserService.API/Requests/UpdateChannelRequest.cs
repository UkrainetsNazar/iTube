namespace UserService.API.Requests;

public sealed record UpdateChannelRequest(
    string Name,
    string? Description,
    string? AvatarBucket,
    string? AvatarKey,
    string? BannerBucket,
    string? BannerKey);