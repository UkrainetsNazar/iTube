using MediatR;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Common;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Commands.UpdateChannelProfile;

public sealed class UpdateChannelProfileCommandHandler(
    IChannelRepository channelRepository,
    IUnitOfWork unitOfWork,
    IConfiguration configuration
) : IRequestHandler<UpdateChannelProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateChannelProfileCommand request, CancellationToken ct)
    {
        var channel = await channelRepository.GetByOwnerIdAsync(UserId.From(request.OwnerId), ct);

        if (channel is null)
            return Result.Failure(Error.NotFound("UpdateChannel.NotFound", "Channel not found."));

        var nameResult = ChannelName.Create(request.Name);
        if (nameResult.IsFailure)
            return Result.Failure(nameResult.Error);

        ChannelDescription? description = null;
        if (request.Description is not null)
        {
            var descResult = ChannelDescription.Create(request.Description);
            if (descResult.IsFailure)
                return Result.Failure(descResult.Error);
            description = descResult.Value;
        }

        var baseUrl = configuration["Storage:PublicBaseUrl"]!.TrimEnd('/');

        MediaReference? avatar = channel.Avatar;
        if (request.AvatarBucket is not null && request.AvatarKey is not null)
        {
            var avatarUrl = $"{baseUrl}/{request.AvatarBucket}/{request.AvatarKey}";
            var avatarResult = MediaReference.Create(request.AvatarBucket, request.AvatarKey, avatarUrl);
            if (avatarResult.IsFailure)
                return Result.Failure(avatarResult.Error);
            avatar = avatarResult.Value;
        }

        MediaReference? banner = channel.Banner;
        if (request.BannerBucket is not null && request.BannerKey is not null)
        {
            var bannerUrl = $"{baseUrl}/{request.BannerBucket}/{request.BannerKey}";
            var bannerResult = MediaReference.Create(request.BannerBucket, request.BannerKey, bannerUrl);
            if (bannerResult.IsFailure)
                return Result.Failure(bannerResult.Error);
            banner = bannerResult.Value;
        }

        var result = channel.UpdateProfile(nameResult.Value, description, avatar, banner);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}