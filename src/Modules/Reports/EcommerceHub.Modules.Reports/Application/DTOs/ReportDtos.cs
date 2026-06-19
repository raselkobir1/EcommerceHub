namespace EcommerceHub.Modules.Reports.Application.DTOs;

public sealed record ReportSummaryDto(
    decimal TotalRevenue,
    int TotalOrders,
    decimal AverageOrderValue,
    int TotalItemsSold,
    int NewCustomers,
    decimal RevenueGrowthPercent);

public sealed record DailyRevenueDto(DateOnly Date, decimal Revenue, int Orders);

public sealed record TopProductDto(Guid ProductId, string ProductName, string Sku, int TotalSold, decimal TotalRevenue);

public sealed record TopCategoryDto(Guid CategoryId, string CategoryName, int TotalOrders, decimal TotalRevenue);
