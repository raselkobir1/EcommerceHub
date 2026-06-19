using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reviews.Application.Commands.SubmitReview;

public sealed record SubmitReviewCommand(
    Guid ProductId,
    Guid CustomerId,
    Guid OrderId,
    int Rating,
    string? Title,
    string? Body) : IRequest<Result<Guid>>;
