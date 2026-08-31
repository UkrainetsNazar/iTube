using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Enums;

namespace VideoService.Application.Commands.ChangeVideoVisibility;

public sealed record ChangeVideoVisibilityCommand(VideoId VideoId, Guid RequestedBy, VideoVisibility Visibility) : IRequest<Result>;