using System.Security.Claims;

namespace EcommerceHub.Modules.Auth.Application.Services;

public interface IJwtService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    DateTime AccessTokenExpiry { get; }
    DateTime RefreshTokenExpiry { get; }
}
