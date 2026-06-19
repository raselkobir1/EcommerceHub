using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.DeleteCoupon;

public sealed record DeleteCouponCommand(Guid Id) : IRequest<Result>;
