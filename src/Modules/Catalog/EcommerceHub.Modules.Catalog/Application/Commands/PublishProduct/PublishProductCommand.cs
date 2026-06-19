using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.PublishProduct;

public sealed record PublishProductCommand(Guid Id) : IRequest<Result>;
