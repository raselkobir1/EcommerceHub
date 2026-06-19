using EcommerceHub.Modules.Auth.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Auth.Application.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken, string? IpAddress)
    : IRequest<Result<AuthTokenDto>>;
