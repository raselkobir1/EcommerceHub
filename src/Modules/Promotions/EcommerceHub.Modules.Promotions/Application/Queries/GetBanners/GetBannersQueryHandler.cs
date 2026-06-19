using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Queries.GetBanners;

internal sealed class GetBannersQueryHandler(IBannerRepository bannerRepository)
    : IRequestHandler<GetBannersQuery, Result<IEnumerable<BannerDto>>>
{
    public async Task<Result<IEnumerable<BannerDto>>> Handle(GetBannersQuery request, CancellationToken ct)
    {
        var banners = await bannerRepository.GetActiveAsync(ct: ct);
        var dtos = banners.Select(MapToDto);
        return Result.Success(dtos);
    }

    private static BannerDto MapToDto(Banner b) => new(
        b.Id,
        b.Title,
        b.ImageUrl,
        b.LinkUrl,
        b.IsActive,
        b.SortOrder,
        b.ActiveFrom,
        b.ActiveTo);
}
