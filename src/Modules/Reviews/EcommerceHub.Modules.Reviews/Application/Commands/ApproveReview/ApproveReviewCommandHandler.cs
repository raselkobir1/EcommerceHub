using EcommerceHub.Modules.Reviews.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reviews.Application.Commands.ApproveReview;

internal sealed class ApproveReviewCommandHandler(
    IProductReviewRepository reviewRepository,
    IReviewsUnitOfWork unitOfWork)
    : IRequestHandler<ApproveReviewCommand, Result>
{
    public async Task<Result> Handle(ApproveReviewCommand request, CancellationToken ct)
    {
        var review = await reviewRepository.GetByIdAsync(request.ReviewId, ct);
        if (review is null)
            return Result.Failure($"Review '{request.ReviewId}' not found.");

        review.Approve();
        reviewRepository.Update(review);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
