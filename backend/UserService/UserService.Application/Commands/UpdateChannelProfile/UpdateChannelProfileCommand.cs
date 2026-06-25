using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;

namespace UserService.Application.Commands.UpdateChannelProfile;

public sealed record UpdateChannelProfileCommand(
    Guid OwnerId,
    string Name,
    string? Description,
    string? AvatarBucket,
    string? AvatarKey,
    string? BannerBucket,
    string? BannerKey) : IRequest<Result>;