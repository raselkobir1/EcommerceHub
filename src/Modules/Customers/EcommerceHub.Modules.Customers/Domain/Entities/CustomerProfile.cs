using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Customers.Domain.Entities;

/// <summary>
/// Denormalised customer profile owned by the Customers module.
/// Seeded from the Auth.CustomerRegisteredEvent; updated independently via UpdateCustomerProfile.
/// </summary>
public sealed class CustomerProfile : AuditableEntity
{
    public string FullName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;

    private CustomerProfile() { }

    public static CustomerProfile Create(Guid id, string fullName, string email, string phone) =>
        new()
        {
            Id = id,
            FullName = fullName,
            Email = email,
            Phone = phone
        };

    public void UpdateProfile(string fullName, string phone)
    {
        FullName = fullName;
        Phone = phone;
    }
}
