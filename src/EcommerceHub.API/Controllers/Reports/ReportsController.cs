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
