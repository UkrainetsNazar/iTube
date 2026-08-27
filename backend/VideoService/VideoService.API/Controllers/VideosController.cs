using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using VideoService.API.Extensions;
using VideoService.API.Requests;
using VideoService.Application.Commands.DeleteVideo;
using VideoService.Application.Commands.PublishVideo;
using VideoService.Application.Commands.ReactToVideo;
using VideoService.Application.Commands.UploadVideo;
using VideoService.Application.Queries.GetVideo;
using VideoService.Application.Queries.GetVideoFeed;
using VideoService.Domain.Enums;

namespace VideoService.API.Controllers;

[ApiController]
[Route("api/videos")]
public sealed class VideosController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromBody] UploadVideoRequest request, CancellationToken ct)
    {
        var authorId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new UploadVideoCommand(request.Title, request.Description, request.Tags, authorId), ct);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPatch("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, [FromBody] PublishVideoRequest request, CancellationToken ct)
    {
        var requestedBy = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        if (!Enum.TryParse<VideoVisibility>(request.Visibility, ignoreCase: true, out var visibility))
            return BadRequest($"Invalid visibility '{request.Visibility}'.");

        var result = await sender.Send(new PublishVideoCommand(new VideoId(id), requestedBy, visibility), ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetVideoQuery(new VideoId(id)), ct);
        return result.ToActionResult();
    }

    [HttpGet("feed")]
    public async Task<IActionResult> Feed([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetVideoFeedQuery(page, pageSize), ct);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var requestedBy = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new DeleteVideoCommand(new VideoId(id), requestedBy), ct);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("{id:guid}/react")]
    public async Task<IActionResult> React(Guid id, [FromBody] ReactToVideoRequest request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        if (!Enum.TryParse<ReactionType>(request.Type, ignoreCase: true, out var type))
            return BadRequest($"Invalid reaction type '{request.Type}'.");

        var result = await sender.Send(new ReactToVideoCommand(new VideoId(id), userId, type), ct);
        return result.ToActionResult();
    }
}