using MediatR;
using Shared.Domain.Common;
using UserService.Application.Interfaces;

namespace UserService.Application.Queries.GetUsersForAdmin;

public sealed class GetUsersForAdminQueryHandler(IUserAccountRepository repository)
    : IRequestHandler<GetUsersForAdminQuery, Result<AdminUserPageDto>>
{
    public async Task<Result<AdminUserPageDto>> Handle(GetUsersForAdminQuery request, CancellationToken ct)
    {
        if (request.Page < 1 || request.PageSize is < 1 or > 100)
            return Result.Failure<AdminUserPageDto>(Error.Validation("AdminUsers.InvalidPaging", "page must be >= 1, pageSize between 1 and 100."));

        var (rows, total) = await repository.GetPagedAsync(request.ChannelName, request.Page, request.PageSize, ct);
        return Result.Success(new AdminUserPageDto(rows, total, request.Page, request.PageSize));
    }
}