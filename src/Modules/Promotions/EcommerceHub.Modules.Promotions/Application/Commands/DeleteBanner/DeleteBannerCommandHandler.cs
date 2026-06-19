using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.DeleteBanner;

internal sealed class DeleteBannerCommandHandler(
    IBannerRepository bannerRepository,
    IPromotionsUnitOfWork unitOfWork)
    : IRequestHandler<DeleteBannerCommand, Result>
{
    public async Task<Result> Handle(DeleteBannerCommand request, CancellationToken ct)
    {
        var banner = await bannerRepository.GetByIdAsync(request.Id, ct);
        if (banner is null)
            return Result.Failure($"Banner '{request.Id}' not found.");

        // Hard delete: banners carry no referential dependencies.
        bannerRepository.Remove(banner);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
