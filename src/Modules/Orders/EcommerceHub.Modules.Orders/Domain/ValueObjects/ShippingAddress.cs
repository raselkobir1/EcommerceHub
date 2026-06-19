using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Orders.Domain.ValueObjects;

public sealed record ShippingAddress
{
    public string Division { get; init; } = default!;
    public string District { get; init; } = default!;
    public string AreaThana { get; init; } = default!;
    public string StreetAddress { get; init; } = default!;
    public string? ApartmentFloor { get; init; }

    public static ShippingAddress Create(string division, string district, string areaThana, string streetAddress, string? apartmentFloor = null)
    {
        if (string.IsNullOrWhiteSpace(division)) throw new DomainException("Division is required.");
        if (string.IsNullOrWhiteSpace(district)) throw new DomainException("District is required.");
        if (string.IsNullOrWhiteSpace(areaThana)) throw new DomainException("Area/Thana is required.");
        if (string.IsNullOrWhiteSpace(streetAddress)) throw new DomainException("Street address is required.");

        return new ShippingAddress
        {
            Division = division,
            District = district,
            AreaThana = areaThana,
            StreetAddress = streetAddress,
            ApartmentFloor = apartmentFloor
        };
    }

    public override string ToString() =>
        $"{StreetAddress}{(ApartmentFloor is not null ? $", {ApartmentFloor}" : "")}, {AreaThana}, {District}, {Division}";
}
