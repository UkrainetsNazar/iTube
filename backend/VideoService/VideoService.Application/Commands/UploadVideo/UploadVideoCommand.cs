using MediatR;
using Shared.Domain.Common;

namespace VideoService.Application.Commands.UploadVideo;

public sealed record UploadVideoCommand(
    string Title, string? Description, List<string> Tags, Guid AuthorId) : IRequest<Result<UploadVideoResponse>>;

public sealed record UploadVideoResponse(Guid VideoId);