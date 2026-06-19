using EcommerceHub.Modules.Reviews.Application.DTOs;
using EcommerceHub.Modules.Reviews.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reviews.Application.Queries.GetProductReviews;

internal sealed class GetProductReviewsQueryHandler(IProductReviewRepository reviewRepository)
    : IRequestHandler<GetProductReviewsQuery, Result<PagedResult<ProductReviewDto>>>
{
    public async Task<Result<PagedResult<ProductReviewDto>>> Handle(
        GetProductReviewsQuery request, CancellationToken ct)
    {
        var reviews = await reviewRepository.GetByProductIdAsync(request.ProductId, ct);

        var reviewList = reviews.ToList();
        var totalCount = reviewList.Count;

        var skip = (request.Page - 1) * request.PageSize;
        var paged = reviewList
            .Skip(skip)
            .Take(request.PageSize)
            .Select(r => new ProductReviewDto(
                r.Id,
                r.ProductId,
                r.CustomerId,
                r.Rating,
                r.Title,
                r.Body,
                r.IsApproved,
                r.IsVerifiedPurchase,
                r.CreatedAt))
            .ToList();

        return Result.Success(PagedResult<ProductReviewDto>.Create(paged, totalCount, request.Page, request.PageSize));
    }
}
