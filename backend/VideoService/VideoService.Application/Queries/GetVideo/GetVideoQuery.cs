using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using VideoService.Application.DTOs;

namespace VideoService.Application.Queries.GetVideo;

public sealed record GetVideoQuery(VideoId VideoId, Guid? RequestedBy) : IRequest<Result<VideoDto>>;