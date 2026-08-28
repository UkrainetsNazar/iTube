using Microsoft.AspNetCore.Builder;
using Shared.Api.Middleware;

namespace Shared.Api.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionMiddleware>();
}