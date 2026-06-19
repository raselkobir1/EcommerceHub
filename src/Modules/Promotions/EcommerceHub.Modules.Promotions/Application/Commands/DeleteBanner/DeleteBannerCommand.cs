using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.DeleteBanner;

public sealed record DeleteBannerCommand(Guid Id) : IRequest<Result>;
