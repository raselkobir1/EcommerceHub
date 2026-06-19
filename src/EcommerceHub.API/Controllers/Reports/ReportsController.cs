using EcommerceHub.Modules.Reports.Application.DTOs;
using EcommerceHub.Modules.Reports.Application.Queries.GetDailyRevenue;
using EcommerceHub.Modules.Reports.Application.Queries.GetReportSummary;
using EcommerceHub.Modules.Reports.Application.Queries.GetTopProducts;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Reports;

[Route("api/reports")]
[Authorize(Policy = "Manager")]
public sealed class ReportsController(MediatR.ISender sender) : ApiController(sender)
{
    /// <summary>
    /// Aggregated report summary: total revenue, orders, items sold, new customers,
    /// and revenue growth vs the equivalent previous period.
    /// Defaults to the last 30 days when no date range is supplied.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<ReportSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(new GetReportSummaryQuery(from, to), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Per-day revenue and order counts for the last N days (default 30, max 365).
    /// Days with zero revenue are included in the response so the chart spine is
    /// always complete.
    /// </summary>
    [HttpGet("daily-revenue")]
    [ProducesResponseType(typeof(ApiResponse<List<DailyRevenueDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDailyRevenue(
        [FromQuery] int days = 30,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(new GetDailyRevenueQuery(days), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Top-selling products ranked by units sold within the optional date range.
    /// Defaults to last 30 days, returns top 10.
    /// </summary>
    [HttpGet("top-products")]
    [ProducesResponseType(typeof(ApiResponse<List<TopProductDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTopProducts(
        [FromQuery] int limit = 10,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(new GetTopProductsQuery(limit, from, to), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Generate a sales report for a given date range.
    /// Returns aggregated revenue, order counts, and top-selling products.
    /// </summary>
    [HttpGet("sales")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetSalesReport(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
        => NotImplementedResponse();

    /// <summary>
    /// Generate a current inventory valuation and stock-level report.
    /// Highlights low-stock and out-of-stock SKUs.
    /// </summary>
    [HttpGet("inventory")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetInventoryReport()
        => NotImplementedResponse();

    /// <summary>
    /// Generate a customer acquisition and retention report.
    /// Includes new registrations, returning customers, and lifetime value summaries.
    /// </summary>
    [HttpGet("customers")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetCustomerReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
        => NotImplementedResponse();

    /// <summary>
    /// Generate an orders report with fulfilment status breakdown.
    /// Supports optional date range filtering.
    /// </summary>
    [HttpGet("orders")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetOrdersReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
        => NotImplementedResponse();
}
