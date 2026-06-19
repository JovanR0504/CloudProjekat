using Microsoft.AspNetCore.Mvc;
using SmartMetering.Domain.Common;

namespace SmartMetering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return HandleError(result.Error!);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return NoContent();

        return HandleError(result.Error!);
    }

    private IActionResult HandleError(Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => BadRequest(new { error.Code, error.Message }),
            ErrorType.NotFound => NotFound(new { error.Code, error.Message }),
            ErrorType.Conflict => Conflict(new { error.Code, error.Message }),
            ErrorType.Unauthorized => Unauthorized(new { error.Code, error.Message }),
            ErrorType.Unexpected => StatusCode(500, new { error.Code, error.Message }),
            _ => StatusCode(500, new { error.Code, error.Message })
        };
    }
}
