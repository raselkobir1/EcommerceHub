using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiController(ISender sender) : ControllerBase
{
    protected ISender Sender { get; } = sender;

    protected IActionResult OkResponse<T>(T data, string? message = null)
        => Ok(ApiResponse<T>.Ok(data, message));

    protected IActionResult CreatedResponse<T>(string routeName, object routeValues, T data)
        => CreatedAtRoute(routeName, routeValues, ApiResponse<T>.Ok(data));

    protected IActionResult BadRequestResponse(string message)
        => BadRequest(ApiResponse<object>.Fail(message));

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<T>.Ok(result.Value));

        return BadRequest(ApiResponse<T>.Fail(result.Error ?? "An error occurred."));
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse.Ok());
        return BadRequest(ApiResponse.Fail(result.Error ?? "An error occurred."));
    }

    protected IActionResult NotImplementedResponse()
        => StatusCode(501, ApiResponse.Fail("This endpoint is not yet implemented."));
}
