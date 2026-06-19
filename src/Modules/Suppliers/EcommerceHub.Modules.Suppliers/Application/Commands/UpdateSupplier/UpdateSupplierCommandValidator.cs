using FluentValidation;

namespace EcommerceHub.Modules.Suppliers.Application.Commands.UpdateSupplier;

internal sealed class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Supplier ID is required.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200).WithMessage("Company name must not exceed 200 characters.");

        RuleFor(x => x.ContactPerson)
            .NotEmpty().WithMessage("Contact person is required.")
            .MaximumLength(200).WithMessage("Contact person name must not exceed 200 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.")
            .When(x => x.Address is not null);

        RuleFor(x => x.BankAccountDetails)
            .MaximumLength(500).WithMessage("Bank account details must not exceed 500 characters.")
            .When(x => x.BankAccountDetails is not null);
    }
}
