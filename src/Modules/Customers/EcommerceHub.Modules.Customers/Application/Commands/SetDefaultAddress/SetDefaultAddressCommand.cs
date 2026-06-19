using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Customers.Application.Commands.SetDefaultAddress;

public sealed record SetDefaultAddressCommand(
    Guid CustomerId,
    Guid AddressId)
    : IRequest<Result<CustomerAddressDto>>;
