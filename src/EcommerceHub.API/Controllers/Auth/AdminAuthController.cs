using EcommerceHub.Modules.Auth.Application.Commands.LoginAdmin;
using EcommerceHub.Modules.Auth.Application.Commands.RefreshToken;
using EcommerceHub.Modules.Auth.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Auth;

[Route("api/auth/admin")]
public sealed class AdminAuthController(
    MediatR.ISender sender,
    IHttpContextAccessor httpContextAccessor) : ApiController(sender)
{
    private string? IpAddress =>
        httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginAdminCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command with { IpAddress = IpAddress }, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<AuthTokenDto>.Ok(result.Value, "Login successful."))
            : BadRequest(ApiResponse<AuthTokenDto>.Fail(result.Error!));
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public IActionResult Logout() => Ok(ApiResponse.Ok("Logged out successfully."));

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command with { IpAddress = IpAddress }, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<AuthTokenDto>.Ok(result.Value))
            : Unauthorized(ApiResponse<AuthTokenDto>.Fail(result.Error!));
    }
}
