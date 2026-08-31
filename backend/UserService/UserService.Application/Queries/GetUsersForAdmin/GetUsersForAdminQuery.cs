using MediatR;
using Shared.Domain.Common;
using UserService.Application.DTOs;

namespace UserService.Application.Queries.GetUsersForAdmin;

public sealed record GetUsersForAdminQuery(string? ChannelName, int Page, int PageSize)
    : IRequest<Result<AdminUserPageDto>>;

public sealed record AdminUserPageDto(IReadOnlyList<UserAccountAdminRow> Items, int TotalCount, int Page, int PageSize);