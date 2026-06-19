using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Modules.Auth.Domain.Events;

namespace EcommerceHub.Modules.Auth.Domain.Entities;

public sealed class Customer : SoftDeletableEntity
{
    public string FullName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public bool IsEmailVerified { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationExpiry { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? BlockReason { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private Customer() { }

    public static Customer Create(string fullName, string email, string phone, string passwordHash)
    {
        var customer = new Customer
        {
            FullName = fullName,
            Email = email.ToLowerInvariant(),
            Phone = phone,
            PasswordHash = passwordHash
        };
        customer.GenerateEmailVerificationToken();
        customer.RaiseDomainEvent(new CustomerRegisteredEvent(
            customer.Id,
            customer.Email,
            customer.EmailVerificationToken!,
            customer.FullName,
            customer.Phone));
        return customer;
    }

    public void GenerateEmailVerificationToken()
    {
        EmailVerificationToken = Guid.NewGuid().ToString("N");
        EmailVerificationExpiry = DateTime.UtcNow.AddHours(24);
    }

    public bool VerifyEmail(string token)
    {
        if (EmailVerificationToken != token ||
            EmailVerificationExpiry < DateTime.UtcNow)
            return false;

        IsEmailVerified = true;
        EmailVerificationToken = null;
        EmailVerificationExpiry = null;
        RaiseDomainEvent(new CustomerEmailVerifiedEvent(Id, Email));
        return true;
    }

    public void UpdateProfile(string fullName, string phone)
    {
        FullName = fullName;
        Phone = phone;
    }

    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;

    public void Block(string reason)
    {
        IsActive = false;
        BlockReason = reason;
    }

    public void Unblock()
    {
        IsActive = true;
        BlockReason = null;
    }

    public void SetPasswordResetToken(string token)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(24);
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
    }

    public void AddRefreshToken(RefreshToken token) => _refreshTokens.Add(token);
}
