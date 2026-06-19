using EcommerceHub.Modules.Reports.Application.DTOs;
using EcommerceHub.Modules.Reports.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Reports.Application.Queries.GetTopProducts;

internal sealed class GetTopProductsQueryHandler(ReportsDbContext db)
    : IRequestHandler<GetTopProductsQuery, Result<List<TopProductDto>>>
{
    public async Task<Result<List<TopProductDto>>> Handle(
        GetTopProductsQuery request, CancellationToken ct)
    {
        var limit = Math.Max(1, Math.Min(request.Limit, 100));
        var to    = (request.To   ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);
        var from  = (request.From ?? DateTime.UtcNow.AddDays(-29)).Date;

        // Join order_items (orders schema) to products (catalog schema) via product_id.
        // ProductName and Sku are denormalized onto order_items so no cross-schema
        // join is needed for the product name — use product_name / sku from order_items.
        var rows = await db.Database
            .SqlQueryRaw<TopProductRow>("""
                SELECT
                    oi.product_id                          AS "ProductId",
                    MAX(oi.product_name)                   AS "ProductName",
                    MAX(oi.sku)                            AS "Sku",
                    SUM(oi.quantity)::int                  AS "TotalSold",
                    COALESCE(SUM(oi.unit_price * oi.quantity), 0) AS "TotalRevenue"
                FROM orders.order_items oi
                INNER JOIN orders.orders o ON o.id = oi.order_id
                WHERE o.created_at >= {0}
                  AND o.created_at <= {1}
                  AND o.payment_status = 'Paid'
                GROUP BY oi.product_id
                ORDER BY "TotalSold" DESC
                LIMIT {2}
                """, from, to, limit)
            .ToListAsync(ct);

        var result = rows
            .Select(r => new TopProductDto(
                ProductId:    r.ProductId,
                ProductName:  r.ProductName,
                Sku:          r.Sku,
                TotalSold:    r.TotalSold,
                TotalRevenue: Math.Round(r.TotalRevenue, 2)))
            .ToList();

        return Result.Success(result);
    }

    private sealed record TopProductRow(
        Guid ProductId,
        string ProductName,
        string Sku,
        int TotalSold,
        decimal TotalRevenue);
}
