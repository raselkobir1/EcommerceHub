using EcommerceHub.Modules.Suppliers.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Suppliers.Application.Commands.CreateSupplier;

public sealed record CreateSupplierCommand(
    string CompanyName,
    string ContactPerson,
    string Phone,
    string Email,
    string? Address,
    string? BankAccountDetails) : IRequest<Result<SupplierDto>>;
