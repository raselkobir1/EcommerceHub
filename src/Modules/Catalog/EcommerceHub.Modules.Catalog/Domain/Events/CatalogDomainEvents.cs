using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Catalog.Domain.Events;

public sealed record ProductCreatedEvent(Guid ProductId, string Name, string Sku) : DomainEvent;
public sealed record ProductPublishedEvent(Guid ProductId, string Name) : DomainEvent;
public sealed record ProductArchivedEvent(Guid ProductId, string Name) : DomainEvent;
public sealed record ProductStockChangedEvent(Guid ProductId, Guid VariantId, int OldStock, int NewStock) : DomainEvent;
