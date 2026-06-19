using EcommerceHub.Modules.Suppliers.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Suppliers.Application.Commands.UpdateSupplier;

public sealed record UpdateSupplierCommand(
    Guid Id,
    string CompanyName,
    string ContactPerson,
    string Phone,
    string Email,
    string? Address,
    string? BankAccountDetails,
    bool IsActive) : IRequest<Result<SupplierDto>>;
