using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;
using VideoService.Application.DTOs;

namespace VideoService.Application.Queries.GetVideo;

public sealed record GetVideoQuery(VideoId VideoId) : IRequest<Result<VideoDto>>;