using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.UpdateBanner;

public sealed record UpdateBannerCommand(
    Guid Id,
    string Title,
    string ImageUrl,
    string? Subtitle,
    string? LinkUrl,
    int SortOrder,
    bool IsActive,
    DateTime? ActiveFrom,
    DateTime? ActiveTo) : IRequest<Result<BannerDto>>;
