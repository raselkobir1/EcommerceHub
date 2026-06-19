using EcommerceHub.Modules.Reports.Application.DTOs;
using EcommerceHub.Modules.Reports.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Reports.Application.Queries.GetReportSummary;

internal sealed class GetReportSummaryQueryHandler(ReportsDbContext db)
    : IRequestHandler<GetReportSummaryQuery, Result<ReportSummaryDto>>
{
    public async Task<Result<ReportSummaryDto>> Handle(
        GetReportSummaryQuery request, CancellationToken ct)
    {
        var to   = (request.To   ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);
        var from = (request.From ?? DateTime.UtcNow.AddDays(-29)).Date;

        // Previous period of same length (for growth calculation)
        var span     = to - from;
        var prevFrom = from - span;
        var prevTo   = from.AddTicks(-1);

        // ── Current period ────────────────────────────────────────────────────
        var current = await db.Database
            .SqlQueryRaw<PeriodAgg>("""
                SELECT
                    COALESCE(SUM(grand_total), 0)       AS "TotalRevenue",
                    COUNT(*)::int                        AS "TotalOrders",
                    COALESCE(SUM(
                        (SELECT COALESCE(SUM(quantity), 0)
                         FROM orders.order_items oi
                         WHERE oi.order_id = o.id)), 0)::int AS "TotalItemsSold"
                FROM orders.orders o
                WHERE o.created_at >= {0}
                  AND o.created_at <= {1}
                  AND o.payment_status = 'Paid'
                """, from, to)
            .FirstOrDefaultAsync(ct)
            ?? new PeriodAgg(0m, 0, 0);

        // ── Previous period (for growth %) ────────────────────────────────────
        var previous = await db.Database
            .SqlQueryRaw<PeriodAgg>("""
                SELECT
                    COALESCE(SUM(grand_total), 0)       AS "TotalRevenue",
                    COUNT(*)::int                        AS "TotalOrders",
                    0::int                               AS "TotalItemsSold"
                FROM orders.orders o
                WHERE o.created_at >= {0}
                  AND o.created_at <= {1}
                  AND o.payment_status = 'Paid'
                """, prevFrom, prevTo)
            .FirstOrDefaultAsync(ct)
            ?? new PeriodAgg(0m, 0, 0);

        // ── New customers registered in the period ────────────────────────────
        var newCustomers = await db.Database
            .SqlQueryRaw<CountResult>("""
                SELECT COUNT(*)::int AS "Value"
                FROM auth.customers
                WHERE created_at >= {0}
                  AND created_at <= {1}
                  AND is_deleted = false
                """, from, to)
            .Select(r => r.Value)
            .FirstOrDefaultAsync(ct);

        var avgOrderValue = current.TotalOrders > 0
            ? current.TotalRevenue / current.TotalOrders
            : 0m;

        var growthPercent = previous.TotalRevenue > 0
            ? Math.Round((current.TotalRevenue - previous.TotalRevenue) / previous.TotalRevenue * 100m, 2)
            : (current.TotalRevenue > 0 ? 100m : 0m);

        return Result.Success(new ReportSummaryDto(
            TotalRevenue:        current.TotalRevenue,
            TotalOrders:         current.TotalOrders,
            AverageOrderValue:   Math.Round(avgOrderValue, 2),
            TotalItemsSold:      current.TotalItemsSold,
            NewCustomers:        newCustomers,
            RevenueGrowthPercent: growthPercent));
    }

    // Keyless result types consumed by SqlQueryRaw
    private sealed record PeriodAgg(decimal TotalRevenue, int TotalOrders, int TotalItemsSold);
    private sealed record CountResult(int Value);
}
