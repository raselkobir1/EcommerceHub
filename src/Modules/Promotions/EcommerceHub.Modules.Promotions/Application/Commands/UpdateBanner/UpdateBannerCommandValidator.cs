using FluentValidation;

namespace EcommerceHub.Modules.Promotions.Application.Commands.UpdateBanner;

internal sealed class UpdateBannerCommandValidator : AbstractValidator<UpdateBannerCommand>
{
    public UpdateBannerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Subtitle)
            .MaximumLength(300)
            .When(x => x.Subtitle is not null);

        RuleFor(x => x.LinkUrl)
            .MaximumLength(500)
            .When(x => x.LinkUrl is not null);

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Sort order must be non-negative.");

        RuleFor(x => x.ActiveTo)
            .GreaterThan(x => x.ActiveFrom)
            .When(x => x.ActiveFrom.HasValue && x.ActiveTo.HasValue)
            .WithMessage("ActiveTo must be after ActiveFrom.");
    }
}
