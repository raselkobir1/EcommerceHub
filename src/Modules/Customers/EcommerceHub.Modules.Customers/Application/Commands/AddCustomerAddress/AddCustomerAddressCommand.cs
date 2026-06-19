using MediatR;
using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Customers.Application.Commands.AddCustomerAddress;

public sealed record AddCustomerAddressCommand(
    Guid CustomerId,
    string Label,
    string Division,
    string District,
    string AreaThana,
    string StreetAddress,
    string? ApartmentFloor,
    bool IsDefault)
    : IRequest<Result<CustomerAddressDto>>;
