using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using Shared.Api.Extensions;
using VideoService.Application.Queries.GetRecommendations;

namespace VideoService.API.Controllers;

[ApiController]
[Route("api/recommendations")]
public sealed class RecommendationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? videoId, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        Guid? userId = null;
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim is not null && Guid.TryParse(claim.Value, out var parsedUserId))
            userId = parsedUserId;

        VideoId? targetVideoId = videoId is null ? null : new VideoId(videoId.Value);
        var result = await sender.Send(new GetRecommendationsQuery(targetVideoId, userId, limit), ct);
        return result.ToActionResult();
    }
}