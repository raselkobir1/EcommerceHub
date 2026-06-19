using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Modules.Auth.Domain.Enums;
using EcommerceHub.Modules.Auth.Domain.Events;

namespace EcommerceHub.Modules.Auth.Domain.Entities;

public sealed class AdminUser : SoftDeletableEntity
{
    public string FullName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public AdminRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorSecret { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? LastLoginIp { get; private set; }
    public int FailedLoginCount { get; private set; }
    public DateTime? LockoutEndAt { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private AdminUser() { }

    public static AdminUser Create(string fullName, string email, string passwordHash, AdminRole role)
    {
        var user = new AdminUser
        {
            FullName = fullName,
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = role
        };
        user.RaiseDomainEvent(new AdminUserCreatedEvent(user.Id, user.Email, user.Role.ToString()));
        return user;
    }

    public void UpdateProfile(string fullName) => FullName = fullName;

    public void ChangeRole(AdminRole role) => Role = role;

    public void RecordLogin(string ipAddress)
    {
        LastLoginAt = DateTime.UtcNow;
        LastLoginIp = ipAddress;
        FailedLoginCount = 0;
        LockoutEndAt = null;
    }

    public void RecordFailedLogin()
    {
        FailedLoginCount++;
        if (FailedLoginCount >= 5)
            LockoutEndAt = DateTime.UtcNow.AddMinutes(15);
    }

    public bool IsLockedOut() =>
        LockoutEndAt.HasValue && LockoutEndAt.Value > DateTime.UtcNow;

    public void SetPasswordResetToken(string token, DateTime dateTime)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiry = dateTime;
    }

    public bool IsPasswordResetTokenValid(string token) =>
        PasswordResetToken == token &&
        PasswordResetTokenExpiry.HasValue &&
        PasswordResetTokenExpiry.Value > DateTime.UtcNow;

    public void ResetPassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
        FailedLoginCount = 0;
        LockoutEndAt = null;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public void AddRefreshToken(RefreshToken token) => _refreshTokens.Add(token);

    public void RevokeAllRefreshTokens(string reason)
    {
        foreach (var t in _refreshTokens.Where(t => t.IsActive))
            t.Revoke(reason, null, null);
    }
}
