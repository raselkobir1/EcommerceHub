using EcommerceHub.Modules.Reports.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reports.Application.Queries.GetReportSummary;

/// <summary>
/// Returns an aggregated summary for the given date range (defaults to the last 30 days).
/// </summary>
public sealed record GetReportSummaryQuery(
    DateTime? From = null,
    DateTime? To = null) : IRequest<Result<ReportSummaryDto>>;
