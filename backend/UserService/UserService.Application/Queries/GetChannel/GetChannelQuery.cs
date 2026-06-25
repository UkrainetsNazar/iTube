using MediatR;
using Shared.Domain.Common;
using UserService.Application.DTOs;

namespace UserService.Application.Queries.GetChannel;

public sealed record GetChannelQuery(Guid ChannelId) : IRequest<Result<ChannelDto>>;