using EcommerceHub.Modules.Payments.Domain.Enums;
using FluentValidation;

namespace EcommerceHub.Modules.Payments.Application.Commands.InitiatePayment;

public sealed class InitiatePaymentCommandValidator : AbstractValidator<InitiatePaymentCommand>
{
    private static readonly string[] ValidMethods =
        Enum.GetNames<PaymentMethod>();

    public InitiatePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(x => x.OrderNumber)
            .NotEmpty().WithMessage("Order number is required.")
            .MaximumLength(30).WithMessage("Order number must not exceed 30 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than zero.");

        RuleFor(x => x.Method)
            .NotEmpty().WithMessage("Payment method is required.")
            .Must(m => ValidMethods.Contains(m, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Payment method must be one of: {string.Join(", ", ValidMethods)}.");

        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("IP address is required.")
            .MaximumLength(45).WithMessage("IP address must not exceed 45 characters.");

        RuleFor(x => x.CustomerPhone)
            .MaximumLength(20).WithMessage("Customer phone must not exceed 20 characters.")
            .When(x => x.CustomerPhone is not null);

        RuleFor(x => x.CustomerEmail)
            .EmailAddress().WithMessage("Customer email must be a valid email address.")
            .MaximumLength(256).WithMessage("Customer email must not exceed 256 characters.")
            .When(x => x.CustomerEmail is not null);
    }
}
