using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Enums;

namespace VideoService.Application.Commands.ReactToVideo;

public sealed record ReactToVideoCommand(VideoId VideoId, Guid UserId, ReactionType Type) : IRequest<Result>;