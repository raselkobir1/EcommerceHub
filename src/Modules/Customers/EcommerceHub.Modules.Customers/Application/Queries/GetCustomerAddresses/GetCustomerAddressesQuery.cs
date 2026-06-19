using MediatR;
using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Customers.Application.Queries.GetCustomerAddresses;

public sealed record GetCustomerAddressesQuery(Guid CustomerId)
    : IRequest<Result<IEnumerable<CustomerAddressDto>>>;
