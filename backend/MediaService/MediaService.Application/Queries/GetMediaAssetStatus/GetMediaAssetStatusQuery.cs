using MediaService.Application.DTO;
using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;

namespace MediaService.Application.Queries.GetMediaAssetStatus;

public sealed record GetMediaAssetStatusQuery(MediaAssetId MediaAssetId) : IRequest<Result<MediaAssetStatusDto>>;