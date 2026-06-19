using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.DeleteProduct;

internal sealed class DeleteProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(request.Id, ct);
        if (product is null)
            return Result.Failure($"Product '{request.Id}' not found.");

        product.SoftDelete("system");

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
