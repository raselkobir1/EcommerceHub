using MediatR;
using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Modules.Customers.Domain.Entities;
using EcommerceHub.Modules.Customers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Customers.Application.Commands.AddCustomerAddress;

internal sealed class AddCustomerAddressCommandHandler(
    ICustomerAddressRepository repository,
    ICustomersUnitOfWork unitOfWork)
    : IRequestHandler<AddCustomerAddressCommand, Result<CustomerAddressDto>>
{
    public async Task<Result<CustomerAddressDto>> Handle(
        AddCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        if (request.IsDefault)
        {
            var existingAddresses = await repository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
            foreach (var existing in existingAddresses)
            {
                if (existing.IsDefault)
                {
                    existing.UnsetDefault();
                    repository.Update(existing);
                }
            }
        }

        var address = CustomerAddress.Create(
            request.CustomerId,
            request.Label,
            request.Division,
            request.District,
            request.AreaThana,
            request.StreetAddress,
            request.ApartmentFloor,
            request.IsDefault);

        await repository.AddAsync(address, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new CustomerAddressDto(
            address.Id,
            address.Label,
            address.Division,
            address.District,
            address.AreaThana,
            address.StreetAddress,
            address.ApartmentFloor,
            address.IsDefault);

        return Result.Success(dto);
    }
}
