using EcommerceHub.Modules.Inventory.Domain.Enums;
using FluentValidation;

namespace EcommerceHub.Modules.Inventory.Application.Commands.AdjustStock;

internal sealed class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.VariantId).NotEmpty();

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name must not be empty.")
            .MaximumLength(300);

        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU must not be empty.")
            .MaximumLength(100);

        RuleFor(x => x.QuantityChange)
            .NotEqual(0)
            .WithMessage("Quantity change must not be zero.");

        RuleFor(x => x.CurrentStock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Current stock cannot be negative.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .Must(BeAValidAdjustmentReason)
            .WithMessage($"Reason must be one of: {string.Join(", ", Enum.GetNames<AdjustmentReason>())}.");

        RuleFor(x => x.AdjustedByUserId)
            .NotEmpty()
            .WithMessage("AdjustedByUserId must not be empty.");
    }

    private static bool BeAValidAdjustmentReason(string reason)
        => Enum.TryParse<AdjustmentReason>(reason, ignoreCase: true, out _);
}
