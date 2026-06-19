using EcommerceHub.Modules.Auth.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Auth.Application.Commands.LoginAdmin;

public sealed record LoginAdminCommand(string Email, string Password, string? IpAddress) : IRequest<Result<AuthTokenDto>>;
