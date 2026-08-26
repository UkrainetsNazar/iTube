using MediatR;
using Shared.Domain.Common;
using Shared.Domain.ValueObjects;

namespace VideoService.Application.Commands.DeleteComment;

public sealed record DeleteCommentCommand(CommentId CommentId, Guid RequestedBy) : IRequest<Result>;