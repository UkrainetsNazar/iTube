using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;

namespace VideoService.Application.Commands.AddComment;

public sealed record AddCommentCommand(VideoId VideoId, Guid AuthorId, string Text) : IRequest<Result<Guid>>;