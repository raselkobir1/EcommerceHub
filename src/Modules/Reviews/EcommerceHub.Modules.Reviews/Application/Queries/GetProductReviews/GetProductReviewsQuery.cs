using EcommerceHub.Modules.Reviews.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reviews.Application.Queries.GetProductReviews;

public sealed record GetProductReviewsQuery(
    Guid ProductId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<ProductReviewDto>>>;
