using EcommerceHub.Modules.Auth.Domain.Entities;
using EcommerceHub.Modules.Auth.Domain.Enums;
using EcommerceHub.Shared.Kernel.Exceptions;
using FluentAssertions;
using Xunit;

namespace EcommerceHub.UnitTests.Domain.Auth;

public sealed class AdminUserTests
{
    [Fact]
    public void Create_ShouldSetCorrectInitialState()
    {
        var user = AdminUser.Create("Admin User", "admin@test.com",
            "hashedpassword", AdminRole.Admin);

        user.FullName.Should().Be("Admin User");
        user.Email.Should().Be("admin@test.com");
        user.Role.Should().Be(AdminRole.Admin);
        user.IsActive.Should().BeTrue();
        user.FailedLoginCount.Should().Be(0);
    }

    [Fact]
    public void RecordFailedLogin_FiveTimes_ShouldLockAccount()
    {
        var user = AdminUser.Create("Admin", "admin@test.com", "hash", AdminRole.Staff);

        for (int i = 0; i < 5; i++)
            user.RecordFailedLogin();

        user.IsLockedOut().Should().BeTrue();
    }

    [Fact]
    public void RecordLogin_ShouldResetFailedAttempts()
    {
        var user = AdminUser.Create("Admin", "admin@test.com", "hash", AdminRole.Staff);
        user.RecordFailedLogin();
        user.RecordFailedLogin();

        user.RecordLogin("127.0.0.1");

        user.FailedLoginCount.Should().Be(0);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var user = AdminUser.Create("Admin", "admin@test.com", "hash", AdminRole.Manager);
        user.Deactivate();

        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void SetPasswordResetToken_ShouldSetTokenAndExpiry()
    {
        var user = AdminUser.Create("Admin", "admin@test.com", "hash", AdminRole.Admin);
        user.SetPasswordResetToken("reset-token-123", DateTime.UtcNow.AddHours(24));

        user.PasswordResetToken.Should().Be("reset-token-123");
        user.IsPasswordResetTokenValid("reset-token-123").Should().BeTrue();
    }

    [Fact]
    public void IsPasswordResetTokenValid_WithExpiredToken_ShouldReturnFalse()
    {
        var user = AdminUser.Create("Admin", "admin@test.com", "hash", AdminRole.Admin);
        user.SetPasswordResetToken("expired-token", DateTime.UtcNow.AddHours(-1));

        user.IsPasswordResetTokenValid("expired-token").Should().BeFalse();
    }

    [Fact]
    public void RaiseDomainEvents_OnCreate_ShouldContainAdminUserCreatedEvent()
    {
        var user = AdminUser.Create("Admin", "admin@test.com", "hash", AdminRole.SuperAdmin);
        user.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "AdminUserCreatedEvent");
    }
}
