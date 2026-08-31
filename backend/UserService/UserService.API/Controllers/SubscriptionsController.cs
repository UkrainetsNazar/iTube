using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Extensions;
using UserService.API.Requests;
using UserService.Application.Commands.Subscribe;
using UserService.Application.Commands.Unsubscribe;
using UserService.Application.Queries.GetUserSubscriptions;
using UserService.Application.Queries.IsSubscribed;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/subscriptions")]
[Authorize]
public sealed class SubscriptionsController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMySubscriptions(CancellationToken ct)
    {
        var subscriberId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new GetUserSubscriptionsQuery(subscriberId), ct);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("check")]
    public async Task<IActionResult> Check([FromQuery] Guid channelId, CancellationToken ct)
    {
        var subscriberId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new IsSubscribedQuery(subscriberId, channelId), ct);
        return result.ToActionResult();
    }

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