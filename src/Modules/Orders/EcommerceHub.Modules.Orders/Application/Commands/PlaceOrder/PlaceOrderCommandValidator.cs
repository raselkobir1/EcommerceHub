using FluentValidation;

namespace EcommerceHub.Modules.Orders.Application.Commands.PlaceOrder;

internal sealed class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    private static readonly string[] AllowedPaymentMethods = ["COD", "BKASH", "NAGAD", "SSLCOMMERZ", "SHURJOPAY", "CARD"];

    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.CustomerPhone)
            .NotEmpty()
            .Matches(@"^(\+8801|01)[3-9]\d{8}$")
            .WithMessage("Invalid Bangladeshi phone number.");
        RuleFor(x => x.Division).NotEmpty().MaximumLength(100);
        RuleFor(x => x.District).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AreaThana).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StreetAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .Must(m => AllowedPaymentMethods.Contains(m.ToUpperInvariant()))
            .WithMessage($"Payment method must be one of: {string.Join(", ", AllowedPaymentMethods)}");
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must contain at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.VariantId).NotEmpty();
            item.RuleFor(i => i.ProductName).NotEmpty().MaximumLength(300);
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(999);
        });
    }
}
