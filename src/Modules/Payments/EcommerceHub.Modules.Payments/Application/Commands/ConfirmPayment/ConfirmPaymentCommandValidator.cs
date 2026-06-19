using FluentValidation;

namespace EcommerceHub.Modules.Payments.Application.Commands.ConfirmPayment;

public sealed class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty().WithMessage("Payment ID is required.");

        RuleFor(x => x.TransactionId)
            .NotEmpty().WithMessage("Transaction ID is required.")
            .MaximumLength(200).WithMessage("Transaction ID must not exceed 200 characters.");

        RuleFor(x => x.GatewayReference)
            .MaximumLength(500).WithMessage("Gateway reference must not exceed 500 characters.")
            .When(x => x.GatewayReference is not null);
    }
}
