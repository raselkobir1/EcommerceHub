using EcommerceHub.Shared.Kernel.Persistence;
using MediatR;

namespace EcommerceHub.Modules.Payments.Infrastructure.Persistence;

internal sealed class PaymentsUnitOfWork(PaymentsDbContext context, IPublisher publisher) : BaseUnitOfWork<PaymentsDbContext>(context, publisher);
