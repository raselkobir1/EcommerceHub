using EcommerceHub.Modules.Reviews.Application.Commands.ApproveReview;
using EcommerceHub.Modules.Reviews.Application.Commands.RejectReview;
using EcommerceHub.Modules.Reviews.Application.Commands.SubmitReview;
using EcommerceHub.Modules.Reviews.Application.DTOs;
using EcommerceHub.Modules.Reviews.Application.Queries.GetProductReviews;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Reviews;

public sealed record SubmitReviewRequest(
    Guid ProductId, Guid OrderId, int Rating, string? Title, string? Body);

[Route("api/reviews")]
public sealed class ReviewsController(MediatR.ISender sender, ICurrentUser currentUser) : ApiController(sender)
{
    [HttpGet("product/{productId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductReviewDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductReviews(
        [FromRoute] Guid productId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(new GetProductReviewsQuery(productId, page, pageSize), ct);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewRequest request, CancellationToken ct)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized(ApiResponse<object>.Fail("Not authenticated."));

        var command = new SubmitReviewCommand(
            request.ProductId, currentUser.UserId.Value, request.OrderId,
            request.Rating, request.Title, request.Body);
        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/approve")]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveReview([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new ApproveReviewCommand(id), ct);
        if (result.IsSuccess)
            return OkResponse<object>(null!, "Review approved.");
        return BadRequest(ApiResponse<object>.Fail(result.Error!));
    }

    [HttpPut("{id:guid}/reject")]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectReview([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new RejectReviewCommand(id), ct);
        if (result.IsSuccess)
            return OkResponse<object>(null!, "Review rejected.");
        return BadRequest(ApiResponse<object>.Fail(result.Error!));
    }
}
