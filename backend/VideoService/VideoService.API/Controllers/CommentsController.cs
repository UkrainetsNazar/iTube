using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Extensions;
using Shared.Domain.ValueObjects;
using VideoService.API.Requests;
using VideoService.Application.Commands.AddComment;
using VideoService.Application.Commands.DeleteComment;
using VideoService.Application.Queries.GetVideoComments;

namespace VideoService.API.Controllers;

[ApiController]
public sealed class CommentsController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost("api/videos/{videoId:guid}/comments")]
    public async Task<IActionResult> Add(Guid videoId, [FromBody] AddCommentRequest request, CancellationToken ct)
    {
        var authorId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new AddCommentCommand(new VideoId(videoId), authorId, request.Text), ct);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpDelete("api/comments/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var requestedBy = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await sender.Send(new DeleteCommentCommand(new CommentId(id), requestedBy), ct);
        return result.ToActionResult();
    }

    [HttpGet("api/videos/{videoId:guid}/comments")]
    public async Task<IActionResult> GetByVideo(
    Guid videoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetVideoCommentsQuery(new VideoId(videoId), page, pageSize), ct);
        return result.ToActionResult();
    }
}