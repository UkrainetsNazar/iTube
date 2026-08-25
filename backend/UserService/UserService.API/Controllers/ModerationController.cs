using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Extensions;
using UserService.API.Requests;
using UserService.Application.Commands.BanUser;
using UserService.Application.Commands.UnbanUser;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/moderation")]
[Authorize(Roles = "Moderator,Admin")]
public sealed class ModerationController(ISender sender) : ControllerBase
{
    [HttpPost("users/{id:guid}/ban")]
    public async Task<IActionResult> BanUser(
        Guid id,
        [FromBody] BanUserRequest request,
        CancellationToken ct)
    {
        var moderatorId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new BanUserCommand(id, moderatorId, request.Reason, request.ExpiresAt), ct);
        return result.ToActionResult();
    }

    [HttpDelete("users/{id:guid}/ban")]
    public async Task<IActionResult> UnbanUser(Guid id, CancellationToken ct)
    {
        var moderatorId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new UnbanUserCommand(id, moderatorId), ct);
        return result.ToActionResult();
    }
}