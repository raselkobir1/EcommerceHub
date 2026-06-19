using System.Security.Claims;
using EcommerceHub.Modules.Auth.Application.DTOs;
using EcommerceHub.Modules.Auth.Application.Services;
using EcommerceHub.Modules.Auth.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using DomainRefreshToken = EcommerceHub.Modules.Auth.Domain.Entities.RefreshToken;

namespace EcommerceHub.Modules.Auth.Application.Commands.LoginCustomer;

internal sealed class LoginCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IAuthUnitOfWork unitOfWork,
    IJwtService jwtService)
    : IRequestHandler<LoginCustomerCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(LoginCustomerCommand request, CancellationToken ct)
    {
        var customer = await customerRepository.GetByEmailAsync(request.Email, ct);

        if (customer is null || !customer.IsActive)
            return Result.Failure<AuthTokenDto>("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
            return Result.Failure<AuthTokenDto>("Invalid credentials.");

        var claims = BuildClaims(customer);
        var accessToken = jwtService.GenerateAccessToken(claims);
        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var refreshToken = DomainRefreshToken.Create(rawRefreshToken, request.IpAddress);

        customer.RecordLogin();
        customer.AddRefreshToken(refreshToken);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthTokenDto(
            accessToken,
            rawRefreshToken,
            jwtService.AccessTokenExpiry,
            jwtService.RefreshTokenExpiry,
            customer.Id.ToString(),
            customer.Email,
            customer.FullName,
            "Customer"));
    }

    private static IEnumerable<Claim> BuildClaims(Domain.Entities.Customer customer) =>
    [
        new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
        new(ClaimTypes.Email, customer.Email),
        new(ClaimTypes.Name, customer.FullName),
        new(ClaimTypes.Role, "Customer"),
        new("portal", "customer")
    ];
}
