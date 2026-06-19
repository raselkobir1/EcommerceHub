using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.UpdateBanner;

internal sealed class UpdateBannerCommandHandler(
    IBannerRepository bannerRepository,
    IPromotionsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateBannerCommand, Result<BannerDto>>
{
    public async Task<Result<BannerDto>> Handle(UpdateBannerCommand request, CancellationToken ct)
    {
        var banner = await bannerRepository.GetByIdAsync(request.Id, ct);
        if (banner is null)
            return Result.Failure<BannerDto>($"Banner '{request.Id}' not found.");

        banner.Update(
            request.Title,
            request.ImageUrl,
            request.Subtitle,
            request.LinkUrl,
            request.SortOrder,
            request.ActiveFrom,
            request.ActiveTo);

        if (request.IsActive)
            banner.Activate();
        else
            banner.Deactivate();

        bannerRepository.Update(banner);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapToDto(banner));
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
