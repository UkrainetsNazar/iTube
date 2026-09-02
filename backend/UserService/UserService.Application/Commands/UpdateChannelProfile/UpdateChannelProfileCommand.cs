using MediatR;
using Shared.Domain.Common;

namespace UserService.Application.Commands.UpdateChannelProfile;

public sealed record UpdateChannelProfileCommand(
    Guid OwnerId,
    string Name,
    string? Description,
    string? AvatarBucket,
    string? AvatarKey,
    string? BannerBucket,
    string? BannerKey) : IRequest<Result>;