using EcommerceHub.Modules.Payments.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Payments.Application.Queries.GetPaymentsByOrder;

public sealed record GetPaymentsByOrderQuery(Guid OrderId) : IRequest<Result<IEnumerable<PaymentSummaryDto>>>;
