using EcommerceHub.Modules.Reviews.Domain.Entities;
using EcommerceHub.Shared.Kernel.Abstractions;

namespace EcommerceHub.Modules.Reviews.Domain.Interfaces;

public interface IProductReviewRepository : IRepository<ProductReview>
{
    Task<IEnumerable<ProductReview>> GetByProductAsync(Guid productId, bool approvedOnly = true, CancellationToken ct = default);
    Task<IEnumerable<ProductReview>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task<double> GetAverageRatingAsync(Guid productId, CancellationToken ct = default);
    Task<bool> HasReviewedAsync(Guid customerId, Guid productId, Guid orderId, CancellationToken ct = default);
    Task<IEnumerable<ProductReview>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<IEnumerable<ProductReview>> GetPendingAsync(CancellationToken ct = default);
}
