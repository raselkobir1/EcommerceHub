using EcommerceHub.Modules.Reports.Application.DTOs;
using EcommerceHub.Modules.Reports.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Reports.Application.Queries.GetDailyRevenue;

internal sealed class GetDailyRevenueQueryHandler(ReportsDbContext db)
    : IRequestHandler<GetDailyRevenueQuery, Result<List<DailyRevenueDto>>>
{
    public async Task<Result<List<DailyRevenueDto>>> Handle(
        GetDailyRevenueQuery request, CancellationToken ct)
    {
        var days = Math.Max(1, Math.Min(request.Days, 365));
        var from = DateTime.UtcNow.Date.AddDays(-(days - 1));
        var to   = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);

        var rows = await db.Database
            .SqlQueryRaw<DailyRevenueRow>("""
                SELECT
                    DATE(created_at AT TIME ZONE 'UTC')   AS "Day",
                    COALESCE(SUM(grand_total), 0)          AS "Revenue",
                    COUNT(*)::int                          AS "Orders"
                FROM orders.orders
                WHERE created_at >= {0}
                  AND created_at <= {1}
                  AND payment_status = 'Paid'
                GROUP BY DATE(created_at AT TIME ZONE 'UTC')
                ORDER BY "Day"
                """, from, to)
            .ToListAsync(ct);

        // Build a complete date spine so days with zero revenue still appear
        var lookup = rows.ToDictionary(r => r.Day, r => r);
        var result = new List<DailyRevenueDto>(days);

        for (var d = from; d <= DateTime.UtcNow.Date; d = d.AddDays(1))
        {
            var date = DateOnly.FromDateTime(d);
            if (lookup.TryGetValue(d, out var row))
                result.Add(new DailyRevenueDto(date, Math.Round(row.Revenue, 2), row.Orders));
            else
                result.Add(new DailyRevenueDto(date, 0m, 0));
        }

        return Result.Success(result);
    }

    private sealed record DailyRevenueRow(DateTime Day, decimal Revenue, int Orders);
}
