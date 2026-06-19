using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Customers.Domain.Entities;

public sealed class CustomerAddress : AuditableEntity
{
    public Guid CustomerId { get; private set; }
    public string Label { get; private set; } = default!;
    public string Division { get; private set; } = default!;
    public string District { get; private set; } = default!;
    public string AreaThana { get; private set; } = default!;
    public string StreetAddress { get; private set; } = default!;
    public string? ApartmentFloor { get; private set; }
    public bool IsDefault { get; private set; }

    private CustomerAddress() { }

    public static CustomerAddress Create(Guid customerId, string label,
        string division, string district, string areaThana,
        string streetAddress, string? apartmentFloor = null, bool isDefault = false) =>
        new()
        {
            CustomerId = customerId, Label = label,
            Division = division, District = district, AreaThana = areaThana,
            StreetAddress = streetAddress, ApartmentFloor = apartmentFloor, IsDefault = isDefault
        };

    public void SetDefault() => IsDefault = true;
    public void UnsetDefault() => IsDefault = false;

    public void Update(string label, string division, string district, string areaThana,
        string streetAddress, string? apartmentFloor)
    {
        Label = label; Division = division; District = district;
        AreaThana = areaThana; StreetAddress = streetAddress; ApartmentFloor = apartmentFloor;
    }
}
