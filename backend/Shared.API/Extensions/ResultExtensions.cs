using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Common;

namespace Shared.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new OkResult();

        return result.Error.Type switch
        {
            ErrorType.Validation => new BadRequestObjectResult(result.Error),
            ErrorType.NotFound => new NotFoundObjectResult(result.Error),
            ErrorType.Conflict => new ConflictObjectResult(result.Error),
            _ => new ObjectResult(result.Error) { StatusCode = 500 }
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return result.Error.Type switch
        {
            ErrorType.Validation => new BadRequestObjectResult(result.Error),
            ErrorType.NotFound => new NotFoundObjectResult(result.Error),
            ErrorType.Conflict => new ConflictObjectResult(result.Error),
            _ => new ObjectResult(result.Error) { StatusCode = 500 }
        };
    }
}