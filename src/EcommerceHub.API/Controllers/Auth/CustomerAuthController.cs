using EcommerceHub.Modules.Auth.Application.Commands.LoginCustomer;
using EcommerceHub.Modules.Auth.Application.Commands.RefreshToken;
using EcommerceHub.Modules.Auth.Application.Commands.RegisterCustomer;
using EcommerceHub.Modules.Auth.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Auth;

[Route("api/auth/customers")]
public sealed class CustomerAuthController(
    MediatR.ISender sender,
    IHttpContextAccessor httpContextAccessor) : ApiController(sender)
{
    private string? IpAddress =>
        httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCustomerCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command with { IpAddress = IpAddress }, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<AuthTokenDto>.Ok(result.Value, "Login successful."))
            : BadRequest(ApiResponse<AuthTokenDto>.Fail(result.Error!));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCustomerCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<CustomerDto>.Ok(result.Value, "Registration successful. Please verify your email."))
            : BadRequest(ApiResponse<CustomerDto>.Fail(result.Error!));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command with { IpAddress = IpAddress }, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<AuthTokenDto>.Ok(result.Value))
            : Unauthorized(ApiResponse<AuthTokenDto>.Fail(result.Error!));
    }

    [HttpPost("logout")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public IActionResult Logout() => Ok(ApiResponse.Ok("Logged out successfully."));
}
