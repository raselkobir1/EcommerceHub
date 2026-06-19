using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using EcommerceHub.Shared.Kernel.Exceptions;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.PublishProduct;

internal sealed class PublishProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<PublishProductCommand, Result>
{
    public async Task<Result> Handle(PublishProductCommand request, CancellationToken ct)
    {
        var product = await productRepository.GetWithDetailsAsync(request.Id, ct);
        if (product is null)
            return Result.Failure($"Product '{request.Id}' not found.");

        try
        {
            product.Publish();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
