using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Queries.GetBanners;

public sealed record GetBannersQuery() : IRequest<Result<IEnumerable<BannerDto>>>;
