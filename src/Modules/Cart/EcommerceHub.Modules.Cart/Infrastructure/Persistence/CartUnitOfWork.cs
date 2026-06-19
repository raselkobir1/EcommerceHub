using EcommerceHub.Modules.Cart.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using MediatR;

namespace EcommerceHub.Modules.Cart.Infrastructure.Persistence;

internal sealed class CartUnitOfWork(CartDbContext context, IPublisher publisher) : BaseUnitOfWork<CartDbContext>(context, publisher), ICartUnitOfWork;
