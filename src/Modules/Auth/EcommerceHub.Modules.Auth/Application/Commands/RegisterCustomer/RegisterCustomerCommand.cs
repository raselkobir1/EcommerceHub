using EcommerceHub.Modules.Auth.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Auth.Application.Commands.RegisterCustomer;

public sealed record RegisterCustomerCommand(
    string FullName,
    string Email,
    string Phone,
    string Password,
    string ConfirmPassword) : IRequest<Result<CustomerDto>>;
