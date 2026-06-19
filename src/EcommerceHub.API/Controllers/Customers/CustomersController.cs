using EcommerceHub.Modules.Customers.Application.Commands.AddCustomerAddress;
using EcommerceHub.Modules.Customers.Application.Commands.SetDefaultAddress;
using EcommerceHub.Modules.Customers.Application.Commands.UpdateCustomerProfile;
using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Modules.Customers.Application.Queries.GetCustomerAddresses;
using EcommerceHub.Modules.Customers.Application.Queries.GetCustomerProfile;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Customers;

public sealed record AddCustomerAddressRequest(
    string Label,
    string Division,
    string District,
    string AreaThana,
    string StreetAddress,
    string? ApartmentFloor,
    bool IsDefault);

public sealed record UpdateCustomerProfileRequest(
    string FullName,
    string Phone);

[Route("api/customers")]
[Authorize]
public sealed class CustomersController(MediatR.ISender sender, ICurrentUser currentUser) : ApiController(sender)
{
    // ── Profile ───────────────────────────────────────────────────────────────

    /// <summary>Get the current customer's profile.</summary>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(ApiResponse<CustomerProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var result = await Sender.Send(
            new GetCustomerProfileQuery(CustomerId: currentUser.UserId!.Value), ct);
        return HandleResult(result);
    }

    /// <summary>Update the current customer's name and phone number.</summary>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(ApiResponse<CustomerProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateCustomerProfileRequest request, CancellationToken ct)
    {
        var command = new UpdateCustomerProfileCommand(
            CustomerId: currentUser.UserId!.Value,
            FullName: request.FullName,
            Phone: request.Phone);

        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }

    // ── Addresses ─────────────────────────────────────────────────────────────

    /// <summary>Get all saved addresses for the current customer.</summary>
    [HttpGet("addresses")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerAddressDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAddresses(CancellationToken ct)
    {
        var result = await Sender.Send(
            new GetCustomerAddressesQuery(CustomerId: currentUser.UserId!.Value), ct);
        return HandleResult(result);
    }

    /// <summary>Add a new delivery address for the current customer.</summary>
    [HttpPost("addresses")]
    [ProducesResponseType(typeof(ApiResponse<CustomerAddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAddress(
        [FromBody] AddCustomerAddressRequest request, CancellationToken ct)
    {
        var command = new AddCustomerAddressCommand(
            CustomerId: currentUser.UserId!.Value,
            Label: request.Label,
            Division: request.Division,
            District: request.District,
            AreaThana: request.AreaThana,
            StreetAddress: request.StreetAddress,
            ApartmentFloor: request.ApartmentFloor,
            IsDefault: request.IsDefault);

        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>Set an existing address as the default delivery address.</summary>
    [HttpPatch("addresses/{addressId:guid}/set-default")]
    [ProducesResponseType(typeof(ApiResponse<CustomerAddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetDefaultAddress([FromRoute] Guid addressId, CancellationToken ct)
    {
        var command = new SetDefaultAddressCommand(
            CustomerId: currentUser.UserId!.Value,
            AddressId: addressId);

        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }
}
