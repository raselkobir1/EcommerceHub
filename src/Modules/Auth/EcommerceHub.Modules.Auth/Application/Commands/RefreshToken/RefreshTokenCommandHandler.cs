using System.Security.Claims;
using EcommerceHub.Modules.Auth.Application.DTOs;
using EcommerceHub.Modules.Auth.Application.Services;
using EcommerceHub.Modules.Auth.Domain.Entities;
using EcommerceHub.Modules.Auth.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using DomainRefreshToken = EcommerceHub.Modules.Auth.Domain.Entities.RefreshToken;

namespace EcommerceHub.Modules.Auth.Application.Commands.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IAdminUserRepository adminUserRepository,
    ICustomerRepository customerRepository,
    IAuthUnitOfWork unitOfWork,
    IJwtService jwtService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var principal = jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
            return Result.Failure<AuthTokenDto>("Invalid access token.");

        var portal = principal.FindFirstValue("portal");
        var email = principal.FindFirstValue(ClaimTypes.Email);

        if (portal == "admin")
            return await HandleAdminRefresh(request, principal, email, ct);

        return await HandleCustomerRefresh(request, principal, email, ct);
    }

    private async Task<Result<AuthTokenDto>> HandleAdminRefresh(
        RefreshTokenCommand request, ClaimsPrincipal principal, string? email, CancellationToken ct)
    {
        var user = await adminUserRepository.GetByRefreshTokenAsync(request.RefreshToken, ct);
        if (user is null || !user.IsActive)
            return Result.Failure<AuthTokenDto>("Invalid refresh token.");

        var oldToken = user.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken);
        if (oldToken is null || !oldToken.IsActive)
            return Result.Failure<AuthTokenDto>("Refresh token is expired or revoked.");

        var newRawToken = jwtService.GenerateRefreshToken();
        oldToken.Revoke("Replaced", request.IpAddress, newRawToken);
        var newRefreshToken = DomainRefreshToken.Create(newRawToken, request.IpAddress);
        user.AddRefreshToken(newRefreshToken);

        var newAccessToken = jwtService.GenerateAccessToken(BuildAdminClaims(user));
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthTokenDto(
            newAccessToken, newRawToken,
            jwtService.AccessTokenExpiry, jwtService.RefreshTokenExpiry,
            user.Id.ToString(), user.Email, user.FullName, user.Role.ToString()));
    }

    private async Task<Result<AuthTokenDto>> HandleCustomerRefresh(
        RefreshTokenCommand request, ClaimsPrincipal principal, string? email, CancellationToken ct)
    {
        var customer = await customerRepository.GetByRefreshTokenAsync(request.RefreshToken, ct);
        if (customer is null || !customer.IsActive)
            return Result.Failure<AuthTokenDto>("Invalid refresh token.");

        var oldToken = customer.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken);
        if (oldToken is null || !oldToken.IsActive)
            return Result.Failure<AuthTokenDto>("Refresh token is expired or revoked.");

        var newRawToken = jwtService.GenerateRefreshToken();
        oldToken.Revoke("Replaced", request.IpAddress, newRawToken);
        var newRefreshToken = DomainRefreshToken.Create(newRawToken, request.IpAddress);
        customer.AddRefreshToken(newRefreshToken);

        var newAccessToken = jwtService.GenerateAccessToken(BuildCustomerClaims(customer));
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthTokenDto(
            newAccessToken, newRawToken,
            jwtService.AccessTokenExpiry, jwtService.RefreshTokenExpiry,
            customer.Id.ToString(), customer.Email, customer.FullName, "Customer"));
    }

    private static IEnumerable<Claim> BuildAdminClaims(AdminUser user) =>
    [
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Name, user.FullName),
        new(ClaimTypes.Role, user.Role.ToString()),
        new("portal", "admin")
    ];

    private static IEnumerable<Claim> BuildCustomerClaims(Customer customer) =>
    [
        new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
        new(ClaimTypes.Email, customer.Email),
        new(ClaimTypes.Name, customer.FullName),
        new(ClaimTypes.Role, "Customer"),
        new("portal", "customer")
    ];
}
