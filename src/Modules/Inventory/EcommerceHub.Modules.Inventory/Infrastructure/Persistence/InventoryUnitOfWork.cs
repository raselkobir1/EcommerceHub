using EcommerceHub.Modules.Inventory.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using MediatR;

namespace EcommerceHub.Modules.Inventory.Infrastructure.Persistence;

internal sealed class InventoryUnitOfWork(InventoryDbContext context, IPublisher publisher) : BaseUnitOfWork<InventoryDbContext>(context, publisher), IInventoryUnitOfWork;
