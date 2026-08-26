using MediaService.Domain.Enums;
using MediatR;
using Shared.Domain.Common;

namespace MediaService.Application.Commands.UploadMedia;

public sealed record UploadMediaCommand(
    Stream Content,
    string OriginalFileName,
    string ContentType,
    MediaType MediaType,
    Guid UploadedBy,
    Guid? VideoId,
    Guid? ChannelId) : IRequest<Result<UploadMediaResponse>>;

public sealed record UploadMediaResponse(Guid MediaAssetId, string Status);