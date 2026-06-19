using EcommerceHub.Modules.Suppliers.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Suppliers.Application.Queries.GetSupplierById;

public sealed record GetSupplierByIdQuery(Guid SupplierId) : IRequest<Result<SupplierDto>>;
