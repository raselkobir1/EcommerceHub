using FluentValidation;

namespace EcommerceHub.Modules.Customers.Application.Commands.SetDefaultAddress;

internal sealed class SetDefaultAddressCommandValidator : AbstractValidator<SetDefaultAddressCommand>
{
    public SetDefaultAddressCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.AddressId)
            .NotEmpty().WithMessage("Address ID is required.");
    }
}
