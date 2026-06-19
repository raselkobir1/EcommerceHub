using EcommerceHub.Modules.Payments.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Payments.Application.Commands.InitiatePayment;

public sealed record InitiatePaymentCommand(
    Guid OrderId,
    string OrderNumber,
    decimal Amount,
    string Method,
    string IpAddress,
    string? CustomerPhone = null,
    string? CustomerEmail = null) : IRequest<Result<InitiatePaymentResponseDto>>;
