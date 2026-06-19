using EcommerceHub.Modules.Customers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using MediatR;

namespace EcommerceHub.Modules.Customers.Infrastructure.Persistence;

internal sealed class CustomersUnitOfWork(CustomersDbContext context, IPublisher publisher) : BaseUnitOfWork<CustomersDbContext>(context, publisher), ICustomersUnitOfWork;
