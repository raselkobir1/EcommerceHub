using FluentValidation;

namespace EcommerceHub.Modules.Catalog.Application.Commands.UpdateProduct;

internal sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(300)
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must be lowercase with hyphens only.");
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.BasePrice).GreaterThan(0).WithMessage("Base price must be positive.");
        RuleFor(x => x.CompareAtPrice).GreaterThan(x => x.BasePrice)
            .When(x => x.CompareAtPrice.HasValue)
            .WithMessage("Compare-at price must be greater than base price.");
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MetaTitle).MaximumLength(70).When(x => x.MetaTitle is not null);
        RuleFor(x => x.MetaDescription).MaximumLength(160).When(x => x.MetaDescription is not null);
    }
}
