using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reviews.Application.Commands.ApproveReview;

public sealed record ApproveReviewCommand(Guid ReviewId) : IRequest<Result>;
