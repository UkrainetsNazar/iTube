using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Enums;

namespace VideoService.Application.Commands.PublishVideo;

public sealed record PublishVideoCommand(VideoId VideoId, Guid RequestedBy, VideoVisibility Visibility) : IRequest<Result>;