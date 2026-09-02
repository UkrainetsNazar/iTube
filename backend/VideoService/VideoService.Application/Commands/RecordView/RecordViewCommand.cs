using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;

namespace VideoService.Application.Commands.RecordView;

public sealed record RecordViewCommand(VideoId VideoId, Guid? UserId, int WatchedSeconds) : IRequest<Result>;