using EcommerceHub.Modules.Reports.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Reports.Application.Queries.GetDailyRevenue;

/// <summary>
/// Returns per-day revenue and order counts for the last <paramref name="Days"/> days.
/// Days defaults to 30.
/// </summary>
public sealed record GetDailyRevenueQuery(int Days = 30) : IRequest<Result<List<DailyRevenueDto>>>;
