using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Extensions;
using UserService.API.Requests;
using UserService.Application.Commands.ChangeRole;
using UserService.Application.Queries.GetUser;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserQuery(id), ct);
        return result.ToActionResult();
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/role")]
    public async Task<IActionResult> ChangeRole(
        Guid id,
        [FromBody] ChangeRoleRequest request,
        CancellationToken ct)
    {
        var changedBy = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new ChangeRoleCommand(id, changedBy, request.Role), ct);
        return result.ToActionResult();
    }
}