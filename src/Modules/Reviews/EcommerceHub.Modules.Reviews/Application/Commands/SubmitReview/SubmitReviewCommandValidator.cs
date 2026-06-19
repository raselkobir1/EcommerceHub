using FluentValidation;

namespace EcommerceHub.Modules.Reviews.Application.Commands.SubmitReview;

internal sealed class SubmitReviewCommandValidator : AbstractValidator<SubmitReviewCommand>
{
    public SubmitReviewCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Title)
            .MaximumLength(150)
            .When(x => x.Title is not null);
        RuleFor(x => x.Body)
            .MaximumLength(2000)
            .When(x => x.Body is not null);
    }
}
