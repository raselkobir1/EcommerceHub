using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.ArchiveProduct;

public sealed record ArchiveProductCommand(Guid Id) : IRequest<Result>;
