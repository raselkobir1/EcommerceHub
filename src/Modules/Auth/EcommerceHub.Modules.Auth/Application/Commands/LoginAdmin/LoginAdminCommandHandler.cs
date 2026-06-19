using System.Security.Claims;
using EcommerceHub.Modules.Auth.Application.DTOs;
using EcommerceHub.Modules.Auth.Application.Services;
using EcommerceHub.Modules.Auth.Domain.Entities;
using EcommerceHub.Modules.Auth.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using DomainRefreshToken = EcommerceHub.Modules.Auth.Domain.Entities.RefreshToken;

namespace EcommerceHub.Modules.Auth.Application.Commands.LoginAdmin;

internal sealed class LoginAdminCommandHandler(
    IAdminUserRepository adminUserRepository,
    IAuthUnitOfWork unitOfWork,
    IJwtService jwtService)
    : IRequestHandler<LoginAdminCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(LoginAdminCommand request, CancellationToken ct)
    {
        var user = await adminUserRepository.GetByEmailAsync(request.Email, ct);

        if (user is null || !user.IsActive || user.IsDeleted)
            return Result.Failure<AuthTokenDto>("Invalid credentials.");

        if (user.IsLockedOut())
            return Result.Failure<AuthTokenDto>("Account is temporarily locked. Please try again later.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Failure<AuthTokenDto>("Invalid credentials.");
        }

        var claims = BuildClaims(user);
        var accessToken = jwtService.GenerateAccessToken(claims);
        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var refreshToken = DomainRefreshToken.Create(rawRefreshToken, request.IpAddress);

        user.RecordLogin(request.IpAddress ?? string.Empty);
        user.AddRefreshToken(refreshToken);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthTokenDto(
            accessToken,
            rawRefreshToken,
            jwtService.AccessTokenExpiry,
            jwtService.RefreshTokenExpiry,
            user.Id.ToString(),
            user.Email,
            user.FullName,
            user.Role.ToString()));
    }

    private static IEnumerable<Claim> BuildClaims(AdminUser user) =>
    [
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Name, user.FullName),
        new(ClaimTypes.Role, user.Role.ToString()),
        new("portal", "admin")
    ];
}
