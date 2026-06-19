using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Suppliers.Application.Commands.DeleteSupplier;

public sealed record DeleteSupplierCommand(Guid SupplierId) : IRequest<Result>;
