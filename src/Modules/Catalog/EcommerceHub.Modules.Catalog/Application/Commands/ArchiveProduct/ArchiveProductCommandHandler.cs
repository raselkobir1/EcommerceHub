using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.ArchiveProduct;

internal sealed class ArchiveProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<ArchiveProductCommand, Result>
{
    public async Task<Result> Handle(ArchiveProductCommand request, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(request.Id, ct);
        if (product is null)
            return Result.Failure($"Product '{request.Id}' not found.");

        product.Archive();

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
