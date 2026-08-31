using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Extensions;
using Shared.Domain.ValueObjects;
using VideoService.API.Requests;
using VideoService.Application.Commands.ChangeVideoVisibility;
using VideoService.Application.Commands.DeleteVideo;
using VideoService.Application.Commands.PublishVideo;
using VideoService.Application.Commands.ReactToVideo;
using VideoService.Application.Commands.RecordView;
using VideoService.Application.Commands.UploadVideo;
using VideoService.Application.Queries.GetLikedVideos;
using VideoService.Application.Queries.GetMyVideos;
using VideoService.Application.Queries.GetSubscriptionsFeed;
using VideoService.Application.Queries.GetVideo;
using VideoService.Application.Queries.GetVideoFeed;
using VideoService.Application.Queries.GetVideosByChannel;
using VideoService.Application.Queries.GetWatchHistory;
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

    [Authorize]
    [HttpPatch("{id:guid}/visibility")]
    public async Task<IActionResult> ChangeVisibility(Guid id, [FromBody] ChangeVisibilityRequest request, CancellationToken ct)
    {
        var requestedBy = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        if (!Enum.TryParse<VideoVisibility>(request.Visibility, ignoreCase: true, out var visibility))
            return BadRequest($"Invalid visibility '{request.Visibility}'.");

        var result = await sender.Send(new ChangeVideoVisibilityCommand(new VideoId(id), requestedBy, visibility), ct);
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

    [Authorize]
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptionsFeed(
    [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new GetSubscriptionsFeedQuery(userId, page, pageSize), ct);
        return result.ToActionResult();
    }

    [HttpGet("/api/channels/{channelId:guid}/videos")]
    public async Task<IActionResult> GetByChannel(
        Guid channelId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetVideosByChannelQuery(channelId, page, pageSize), ct);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new GetWatchHistoryQuery(userId, page, pageSize), ct);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/views")]
    public async Task<IActionResult> RecordView(Guid id, CancellationToken ct)
    {
        Guid? userId = null;
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim is not null && Guid.TryParse(claim.Value, out var parsed))
            userId = parsed;

        var result = await sender.Send(new RecordViewCommand(new VideoId(id), userId), ct);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("liked")]
    public async Task<IActionResult> GetLiked(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new GetLikedVideosQuery(userId, page, pageSize), ct);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("/api/users/me/videos")]
    public async Task<IActionResult> GetMyVideos(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var authorId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new GetMyVideosQuery(authorId, page, pageSize), ct);
        return result.ToActionResult();
    }
}