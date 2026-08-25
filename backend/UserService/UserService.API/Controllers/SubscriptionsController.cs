using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Extensions;
using UserService.API.Requests;
using UserService.Application.Commands.Subscribe;
using UserService.Application.Commands.Unsubscribe;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/subscriptions")]
[Authorize]
public sealed class SubscriptionsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Subscribe(
        [FromBody] SubscribeRequest request,
        CancellationToken ct)
    {
        var subscriberId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new SubscribeCommand(subscriberId, request.TargetChannelId), ct);
        return result.ToActionResult();
    }

    [HttpDelete]
    public async Task<IActionResult> Unsubscribe(
        [FromBody] UnsubscribeRequest request,
        CancellationToken ct)
    {
        var subscriberId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new UnsubscribeCommand(subscriberId, request.TargetChannelId), ct);
        return result.ToActionResult();
    }
}