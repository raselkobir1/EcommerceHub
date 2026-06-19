namespace EcommerceHub.Modules.Suppliers.Application.DTOs;

public sealed record SupplierDto(
    Guid Id,
    string CompanyName,
    string ContactPerson,
    string Phone,
    string Email,
    string? Address,
    string? BankAccountDetails,
    bool IsActive,
    int TotalPurchaseOrders,
    decimal TotalOwed,
    decimal TotalPaid,
    decimal OutstandingBalance,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record SupplierListDto(
    Guid Id,
    string CompanyName,
    string ContactPerson,
    string Phone,
    string Email,
    bool IsActive,
    int TotalPurchaseOrders,
    decimal OutstandingBalance,
    DateTime CreatedAt);
