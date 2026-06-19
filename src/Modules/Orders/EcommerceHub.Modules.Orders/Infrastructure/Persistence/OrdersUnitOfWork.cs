using EcommerceHub.Modules.Orders.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using MediatR;

namespace EcommerceHub.Modules.Orders.Infrastructure.Persistence;

internal sealed class OrdersUnitOfWork(OrdersDbContext context, IPublisher publisher) : BaseUnitOfWork<OrdersDbContext>(context, publisher), IOrdersUnitOfWork;
