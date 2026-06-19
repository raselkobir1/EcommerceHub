using FluentValidation;

namespace EcommerceHub.Modules.Catalog.Application.Commands.CreateProduct;

internal sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(300)
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must be lowercase with hyphens only.");
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.BasePrice).GreaterThan(0).WithMessage("Base price must be positive.");
        RuleFor(x => x.CompareAtPrice).GreaterThan(x => x.BasePrice)
            .When(x => x.CompareAtPrice.HasValue)
            .WithMessage("Compare-at price must be greater than base price.");
        RuleFor(x => x.MetaTitle).MaximumLength(70).When(x => x.MetaTitle is not null);
        RuleFor(x => x.MetaDescription).MaximumLength(160).When(x => x.MetaDescription is not null);

        RuleForEach(x => x.Variants).ChildRules(v =>
        {
            v.RuleFor(r => r.Sku).NotEmpty().MaximumLength(100);
            v.RuleFor(r => r.Price).GreaterThan(0);
            v.RuleFor(r => r.StockQuantity).GreaterThanOrEqualTo(0);
        });
    }
}
