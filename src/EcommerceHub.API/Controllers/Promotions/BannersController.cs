using EcommerceHub.Modules.Promotions.Application.Commands.DeleteBanner;
using EcommerceHub.Modules.Promotions.Application.Commands.UpdateBanner;
using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Modules.Promotions.Application.Queries.GetBanners;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Promotions;

[Route("api/banners")]
public sealed class BannersController(MediatR.ISender sender) : ApiController(sender)
{
    // GET api/banners
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BannerDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBanners(CancellationToken ct)
    {
        var result = await Sender.Send(new GetBannersQuery(), ct);
        return HandleResult(result);
    }

    // PUT api/banners/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<BannerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBanner(Guid id, [FromBody] UpdateBannerCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequestResponse("Route id does not match the command id.");

        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }

    // DELETE api/banners/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBanner(Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new DeleteBannerCommand(id), ct);
        return HandleResult(result);
    }
}
