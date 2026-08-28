using MediaService.Application.Commands.UploadMedia;
using MediaService.API.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Extensions;

namespace MediaService.API.Controllers;

[ApiController]
[Route("api/media")]
[Authorize]
public sealed class MediaController(ISender sender) : ControllerBase
{
    [HttpPost("upload")]
    [RequestSizeLimit(500_000_000)]
    public async Task<IActionResult> Upload([FromForm] UploadMediaRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<Domain.Enums.MediaType>(request.MediaType, ignoreCase: true, out var mediaType))
            return BadRequest($"Invalid mediaType '{request.MediaType}'. Expected RawVideo, Avatar, or Banner.");

        var uploadedBy = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        await using var stream = request.File.OpenReadStream();

        var result = await sender.Send(new UploadMediaCommand(
            stream,
            request.File.FileName,
            request.File.ContentType,
            mediaType,
            uploadedBy,
            request.VideoId,
            request.ChannelId), ct);

        return result.ToActionResult();
    }
}