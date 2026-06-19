using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Auth.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public string? RevokedReason { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string? CreatedByIp { get; private set; }
    public string? RevokedByIp { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { }

    public static RefreshToken Create(string token, string? createdByIp, int expiryDays = 7)
    {
        return new RefreshToken
        {
            Token = token,
            CreatedByIp = createdByIp,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };
    }

    public void Revoke(string reason, string? revokedByIp, string? replacedByToken)
    {
        IsRevoked = true;
        RevokedReason = reason;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
    }
}
