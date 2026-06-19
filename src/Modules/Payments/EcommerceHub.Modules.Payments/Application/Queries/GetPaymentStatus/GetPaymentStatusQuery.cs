using EcommerceHub.Modules.Payments.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Payments.Application.Queries.GetPaymentStatus;

public sealed record GetPaymentStatusQuery(Guid PaymentId) : IRequest<Result<PaymentStatusDto>>;
