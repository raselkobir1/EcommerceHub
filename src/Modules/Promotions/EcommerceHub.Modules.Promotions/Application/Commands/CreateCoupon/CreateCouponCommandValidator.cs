using EcommerceHub.Modules.Promotions.Domain.Enums;
using FluentValidation;

namespace EcommerceHub.Modules.Promotions.Application.Commands.CreateCoupon;

internal sealed class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    private static readonly string[] ValidTypes =
        Enum.GetNames<CouponType>();

    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[A-Za-z0-9_\-]+$")
            .WithMessage("Coupon code may only contain letters, digits, hyphens, and underscores.");

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => ValidTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Type must be one of: {string.Join(", ", ValidTypes)}.");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Value must be positive.");

        RuleFor(x => x.Value)
            .LessThanOrEqualTo(100)
            .When(x => string.Equals(x.Type, nameof(CouponType.Percentage), StringComparison.OrdinalIgnoreCase))
            .WithMessage("Percentage coupon value cannot exceed 100.");

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.ExpiryDate.HasValue)
            .WithMessage("Expiry date must be after start date.");

        RuleFor(x => x.MinimumOrderAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinimumOrderAmount.HasValue)
            .WithMessage("Minimum order amount must be non-negative.");

        RuleFor(x => x.TotalUsageLimit)
            .GreaterThan(0)
            .When(x => x.TotalUsageLimit.HasValue)
            .WithMessage("Total usage limit must be positive.");

        RuleFor(x => x.PerCustomerLimit)
            .GreaterThan(0)
            .When(x => x.PerCustomerLimit.HasValue)
            .WithMessage("Per-customer limit must be positive.");
    }
}
