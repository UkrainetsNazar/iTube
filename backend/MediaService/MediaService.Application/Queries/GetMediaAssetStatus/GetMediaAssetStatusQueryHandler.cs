using MediatR;
using MediaService.Application.Interfaces;
using Shared.Domain.Common;
using MediaService.Application.DTO;

namespace MediaService.Application.Queries.GetMediaAssetStatus;

public sealed class GetMediaAssetStatusQueryHandler(IMediaAssetRepository repository)
    : IRequestHandler<GetMediaAssetStatusQuery, Result<MediaAssetStatusDto>>
{
    public async Task<Result<MediaAssetStatusDto>> Handle(GetMediaAssetStatusQuery request, CancellationToken ct)
    {
        var asset = await repository.GetByIdAsync(request.MediaAssetId, ct);
        if (asset is null)
            return Result.Failure<MediaAssetStatusDto>(Error.NotFound("MediaAsset.NotFound", "Media asset not found."));

        return Result.Success(new MediaAssetStatusDto(asset.Id.Value, asset.Status.ToString(), asset.FailureReason));
    }
}