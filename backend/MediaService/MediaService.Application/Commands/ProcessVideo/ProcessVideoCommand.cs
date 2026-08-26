using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;

namespace MediaService.Application.Commands.ProcessVideo;

public sealed record ProcessVideoCommand(MediaAssetId MediaAssetId) : IRequest<Result>;