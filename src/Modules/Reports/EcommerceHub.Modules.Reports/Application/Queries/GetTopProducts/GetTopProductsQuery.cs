using EcommerceHub.Modules.Reports.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reports.Application.Queries.GetTopProducts;

/// <summary>
/// Returns the top-selling products ranked by total units sold within the
/// optional date range. Defaults to the last 30 days, top 10 products.
/// </summary>
public sealed record GetTopProductsQuery(
    int Limit = 10,
    DateTime? From = null,
    DateTime? To = null) : IRequest<Result<List<TopProductDto>>>;
