namespace EcommerceHub.Modules.Customers.Application.DTOs;

public sealed record CustomerProfileDto(
    Guid Id,
    string FullName,
    string Email,
    string Phone);

public sealed record CustomerAddressDto(
    Guid Id,
    string Label,
    string Division,
    string District,
    string AreaThana,
    string StreetAddress,
    string? ApartmentFloor,
    bool IsDefault);
