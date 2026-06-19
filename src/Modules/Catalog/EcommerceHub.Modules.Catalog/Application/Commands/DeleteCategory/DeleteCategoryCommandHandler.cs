using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, ct);
        if (category is null)
            return Result.Failure($"Category '{request.Id}' not found.");

        var hasChildren = await categoryRepository.ExistsAsync(c => c.ParentId == request.Id, ct);
        if (hasChildren)
            return Result.Failure("Cannot delete a category that has child categories. Remove or reassign children first.");

        var hasProducts = await categoryRepository.ExistsAsync(c => c.Id == request.Id && c.Products.Any(), ct);
        if (hasProducts)
            return Result.Failure("Cannot delete a category that has products assigned to it.");

        category.SoftDelete("system");

        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
