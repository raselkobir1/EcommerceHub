namespace EcommerceHub.Modules.Auth.Application.DTOs;

public sealed record AuthTokenDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry,
    DateTime RefreshTokenExpiry,
    string UserId,
    string Email,
    string FullName,
    string Role);

public sealed record AdminUserDto(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    bool IsActive,
    DateTime? LastLoginAt,
    DateTime CreatedAt);

public sealed record CustomerDto(
    Guid Id,
    string FullName,
    string Email,
    string Phone,
    bool IsEmailVerified,
    bool IsActive,
    DateTime? LastLoginAt,
    DateTime CreatedAt);
