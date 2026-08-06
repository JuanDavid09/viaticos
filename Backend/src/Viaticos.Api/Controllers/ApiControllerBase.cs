using Microsoft.AspNetCore.Mvc;
using Viaticos.Application.Common.Models;

namespace Viaticos.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        return ToProblem(result.ErrorCode, result.Error);
    }

    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return ToProblem(result.ErrorCode, result.Error);
    }

    private IActionResult ToProblem(string? code, string? message)
    {
        return code switch
        {
            "NOT_FOUND" => NotFound(new { code, message }),
            "FORBIDDEN" => StatusCode(StatusCodes.Status403Forbidden, new { code, message }),
            "NO_EDITABLE" or "ESTADO_INVALIDO" => Conflict(new { code, message }),
            "VALIDATION_ERROR" => BadRequest(new { code, message }),
            _ => BadRequest(new { code, message })
        };
    }
}
