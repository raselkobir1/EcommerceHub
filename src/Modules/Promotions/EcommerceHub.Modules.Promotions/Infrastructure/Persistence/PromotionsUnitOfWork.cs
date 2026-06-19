using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Infrastructure.Persistence;

internal sealed class PromotionsUnitOfWork(PromotionsDbContext context, IPublisher publisher) : BaseUnitOfWork<PromotionsDbContext>(context, publisher), IPromotionsUnitOfWork;
