using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Customers.Application.Commands.UpdateCustomerProfile;

public sealed record UpdateCustomerProfileCommand(
    Guid CustomerId,
    string FullName,
    string Phone)
    : IRequest<Result<CustomerProfileDto>>;
