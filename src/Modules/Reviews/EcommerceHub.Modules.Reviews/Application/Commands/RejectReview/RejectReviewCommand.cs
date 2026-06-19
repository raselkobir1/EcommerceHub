using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reviews.Application.Commands.RejectReview;

public sealed record RejectReviewCommand(Guid ReviewId) : IRequest<Result>;
