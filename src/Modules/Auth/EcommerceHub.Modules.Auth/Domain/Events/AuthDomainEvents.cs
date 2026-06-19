using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Auth.Domain.Events;

public sealed record AdminUserCreatedEvent(Guid UserId, string Email, string Role) : DomainEvent;

public sealed record CustomerRegisteredEvent(
    Guid CustomerId,
    string Email,
    string VerificationToken,
    string FullName,
    string Phone) : DomainEvent;

public sealed record CustomerEmailVerifiedEvent(Guid CustomerId, string Email) : DomainEvent;

public sealed record PasswordResetRequestedEvent(Guid UserId, string Email, string ResetToken) : DomainEvent;
