using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;

namespace VideoService.Application.Commands.DeleteVideo;

public sealed record DeleteVideoCommand(VideoId VideoId, Guid RequestedBy) : IRequest<Result>;