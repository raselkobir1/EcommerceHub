using EcommerceHub.Modules.Reviews.Domain.Entities;
using EcommerceHub.Modules.Reviews.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reviews.Application.Commands.SubmitReview;

internal sealed class SubmitReviewCommandHandler(
    IProductReviewRepository reviewRepository,
    IReviewsUnitOfWork unitOfWork)
    : IRequestHandler<SubmitReviewCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SubmitReviewCommand request, CancellationToken ct)
    {
        if (await reviewRepository.HasReviewedAsync(request.CustomerId, request.ProductId, request.OrderId, ct))
            return Result.Failure<Guid>("You have already submitted a review for this order item.");

        var review = ProductReview.Create(
            request.ProductId,
            request.CustomerId,
            request.OrderId,
            request.Rating,
            request.Title,
            request.Body);

        await reviewRepository.AddAsync(review, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(review.Id);
    }
}
