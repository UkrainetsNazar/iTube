using MediatR;
using Shared.Domain.Common;
using VideoService.Application.DTOs;

namespace VideoService.Application.Queries.GetVideosByChannel;

public sealed record GetVideosByChannelQuery(Guid ChannelId, int Page, int PageSize) : IRequest<Result<IReadOnlyList<VideoDto>>>;