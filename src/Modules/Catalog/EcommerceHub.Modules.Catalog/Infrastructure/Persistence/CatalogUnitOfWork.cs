using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Infrastructure.Persistence;

internal sealed class CatalogUnitOfWork(CatalogDbContext context, IPublisher publisher) : BaseUnitOfWork<CatalogDbContext>(context, publisher), ICatalogUnitOfWork;
