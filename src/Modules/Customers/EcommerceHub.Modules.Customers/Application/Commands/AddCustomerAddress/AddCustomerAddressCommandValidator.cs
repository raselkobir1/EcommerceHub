using FluentValidation;

namespace EcommerceHub.Modules.Customers.Application.Commands.AddCustomerAddress;

internal sealed class AddCustomerAddressCommandValidator : AbstractValidator<AddCustomerAddressCommand>
{
    public AddCustomerAddressCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("Label is required.")
            .MaximumLength(100).WithMessage("Label must not exceed 100 characters.");

        RuleFor(x => x.Division)
            .NotEmpty().WithMessage("Division is required.")
            .MaximumLength(100).WithMessage("Division must not exceed 100 characters.");

        RuleFor(x => x.District)
            .NotEmpty().WithMessage("District is required.")
            .MaximumLength(100).WithMessage("District must not exceed 100 characters.");

        RuleFor(x => x.AreaThana)
            .NotEmpty().WithMessage("Area/Thana is required.")
            .MaximumLength(100).WithMessage("Area/Thana must not exceed 100 characters.");

        RuleFor(x => x.StreetAddress)
            .NotEmpty().WithMessage("Street address is required.")
            .MaximumLength(250).WithMessage("Street address must not exceed 250 characters.");

        RuleFor(x => x.ApartmentFloor)
            .MaximumLength(100).WithMessage("Apartment/Floor must not exceed 100 characters.")
            .When(x => x.ApartmentFloor is not null);
    }
}
