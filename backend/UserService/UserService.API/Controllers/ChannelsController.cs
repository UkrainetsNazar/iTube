using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Extensions;
using UserService.API.Requests;
using UserService.Application.Commands.UpdateChannelProfile;
using UserService.Application.Queries.GetChannel;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/channels")]
public sealed class ChannelsController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetChannel(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetChannelQuery(id), ct);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateChannel(
        Guid id,
        [FromBody] UpdateChannelRequest request,
        CancellationToken ct)
    {
        var ownerId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        if (ownerId != id)
            return Forbid();

        var result = await sender.Send(new UpdateChannelProfileCommand(
            ownerId,
            request.Name,
            request.Description,
            request.AvatarBucket,
            request.AvatarKey,
            request.BannerBucket,
            request.BannerKey), ct);

        return result.ToActionResult();
    }
}