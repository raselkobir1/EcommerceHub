using EcommerceHub.Modules.Auth.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Auth.Application.Commands.LoginCustomer;

public sealed record LoginCustomerCommand(string Email, string Password, string? IpAddress) : IRequest<Result<AuthTokenDto>>;
