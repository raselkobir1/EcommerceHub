using EcommerceHub.Shared.Kernel.Persistence;
using MediatR;

namespace EcommerceHub.Modules.Suppliers.Infrastructure.Persistence;

internal sealed class SuppliersUnitOfWork(SuppliersDbContext context, IPublisher publisher) : BaseUnitOfWork<SuppliersDbContext>(context, publisher);
