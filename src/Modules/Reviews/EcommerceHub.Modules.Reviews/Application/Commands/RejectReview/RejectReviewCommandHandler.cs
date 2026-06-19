using EcommerceHub.Modules.Reviews.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reviews.Application.Commands.RejectReview;

internal sealed class RejectReviewCommandHandler(
    IProductReviewRepository reviewRepository,
    IReviewsUnitOfWork unitOfWork)
    : IRequestHandler<RejectReviewCommand, Result>
{
    public async Task<Result> Handle(RejectReviewCommand request, CancellationToken ct)
    {
        var review = await reviewRepository.GetByIdAsync(request.ReviewId, ct);
        if (review is null)
            return Result.Failure($"Review '{request.ReviewId}' not found.");

        review.Reject();
        reviewRepository.Update(review);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
