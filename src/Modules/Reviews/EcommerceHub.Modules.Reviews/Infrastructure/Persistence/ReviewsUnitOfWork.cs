using EcommerceHub.Modules.Reviews.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using MediatR;

namespace EcommerceHub.Modules.Reviews.Infrastructure.Persistence;

internal sealed class ReviewsUnitOfWork(ReviewsDbContext context, IPublisher publisher) : BaseUnitOfWork<ReviewsDbContext>(context, publisher), IReviewsUnitOfWork;
