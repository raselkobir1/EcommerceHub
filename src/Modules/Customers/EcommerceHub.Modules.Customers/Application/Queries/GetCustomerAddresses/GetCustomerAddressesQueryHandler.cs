using MediatR;
using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Modules.Customers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Customers.Application.Queries.GetCustomerAddresses;

internal sealed class GetCustomerAddressesQueryHandler(ICustomerAddressRepository repository)
    : IRequestHandler<GetCustomerAddressesQuery, Result<IEnumerable<CustomerAddressDto>>>
{
    public async Task<Result<IEnumerable<CustomerAddressDto>>> Handle(
        GetCustomerAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await repository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        var dtos = addresses.Select(a => new CustomerAddressDto(
            a.Id,
            a.Label,
            a.Division,
            a.District,
            a.AreaThana,
            a.StreetAddress,
            a.ApartmentFloor,
            a.IsDefault));

        return Result.Success(dtos);
    }
}
