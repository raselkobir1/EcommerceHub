using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Customers.Application.Queries.GetCustomerProfile;

public sealed record GetCustomerProfileQuery(Guid CustomerId)
    : IRequest<Result<CustomerProfileDto>>;
