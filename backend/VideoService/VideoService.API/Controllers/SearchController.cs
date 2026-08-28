using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Extensions;
using VideoService.Application.Queries.SearchVideos;

namespace VideoService.API.Controllers;

[ApiController]
[Route("api/videos/search")]
public sealed class SearchController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? q, [FromQuery] string? tags,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var tagList = string.IsNullOrWhiteSpace(tags)
            ? null
            : tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        var result = await sender.Send(new SearchVideosQuery(q, tagList, page, pageSize), ct);
        return result.ToActionResult();
    }
}