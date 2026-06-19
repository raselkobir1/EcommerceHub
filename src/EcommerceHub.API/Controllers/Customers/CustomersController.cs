using EcommerceHub.Modules.Customers.Application.Commands.AddCustomerAddress;
using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Modules.Customers.Application.Queries.GetCustomerAddresses;
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

[Route("api/customers")]
[Authorize]
public sealed class CustomersController(MediatR.ISender sender, ICurrentUser currentUser) : ApiController(sender)
{
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
    public async Task<IActionResult> AddAddress([FromBody] AddCustomerAddressRequest request, CancellationToken ct)
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
}
