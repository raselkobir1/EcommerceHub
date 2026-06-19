using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Suppliers.Domain.Entities;

public sealed class Supplier : SoftDeletableEntity
{
    public string CompanyName { get; private set; } = default!;
    public string ContactPerson { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Address { get; private set; }
    public string? BankAccountDetails { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<PurchaseOrder> _purchaseOrders = [];
    public IReadOnlyCollection<PurchaseOrder> PurchaseOrders => _purchaseOrders.AsReadOnly();

    private Supplier() { }

    public static Supplier Create(string companyName, string contactPerson,
        string phone, string email, string? address = null, string? bankAccountDetails = null) =>
        new()
        {
            CompanyName = companyName, ContactPerson = contactPerson,
            Phone = phone, Email = email.ToLowerInvariant(),
            Address = address, BankAccountDetails = bankAccountDetails
        };

    public void Update(string companyName, string contactPerson, string phone,
        string email, string? address, string? bankAccountDetails, bool isActive)
    {
        CompanyName = companyName; ContactPerson = contactPerson;
        Phone = phone; Email = email.ToLowerInvariant();
        Address = address; BankAccountDetails = bankAccountDetails;
        IsActive = isActive;
    }

    public decimal TotalOwed => _purchaseOrders
        .Where(po => po.IsReceived)
        .Sum(po => po.TotalAmount);

    public decimal TotalPaid => _purchaseOrders
        .SelectMany(po => po.Payments)
        .Sum(p => p.Amount);

    public decimal OutstandingBalance => TotalOwed - TotalPaid;
}
