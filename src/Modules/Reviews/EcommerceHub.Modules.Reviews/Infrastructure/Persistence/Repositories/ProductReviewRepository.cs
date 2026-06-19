using EcommerceHub.Modules.Reviews.Domain.Entities;
using EcommerceHub.Modules.Reviews.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Reviews.Infrastructure.Persistence.Repositories;

internal sealed class ProductReviewRepository(ReviewsDbContext context)
    : BaseRepository<ProductReview, ReviewsDbContext>(context), IProductReviewRepository
{
    public async Task<IEnumerable<ProductReview>> GetByProductAsync(Guid productId, bool approvedOnly = true, CancellationToken ct = default)
    {
        var query = context.ProductReviews.Where(r => r.ProductId == productId);
        if (approvedOnly) query = query.Where(r => r.IsApproved);
        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync(ct);
    }

    public async Task<IEnumerable<ProductReview>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default)
        => await context.ProductReviews.Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt).ToListAsync(ct);

    public async Task<double> GetAverageRatingAsync(Guid productId, CancellationToken ct = default)
    {
        var reviews = await context.ProductReviews
            .Where(r => r.ProductId == productId && r.IsApproved).ToListAsync(ct);
        return reviews.Count == 0 ? 0 : reviews.Average(r => r.Rating);
    }

    public async Task<bool> HasReviewedAsync(Guid customerId, Guid productId, Guid orderId, CancellationToken ct = default)
        => await context.ProductReviews.AnyAsync(r => r.CustomerId == customerId &&
                                                       r.ProductId == productId &&
                                                       r.OrderId == orderId, ct);

    public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
        => await context.ProductReviews
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<ProductReview>> GetPendingAsync(CancellationToken ct = default)
        => await context.ProductReviews
            .Where(r => !r.IsApproved)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(ct);
}
