using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Modules.Customers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Customers.Application.Commands.SetDefaultAddress;

internal sealed class SetDefaultAddressCommandHandler(
    ICustomerAddressRepository repository,
    ICustomersUnitOfWork unitOfWork)
    : IRequestHandler<SetDefaultAddressCommand, Result<CustomerAddressDto>>
{
    public async Task<Result<CustomerAddressDto>> Handle(
        SetDefaultAddressCommand request, CancellationToken cancellationToken)
    {
        var target = await repository.GetByIdAsync(request.AddressId, cancellationToken);

        if (target is null)
            return Result.Failure<CustomerAddressDto>("Address not found.");

        if (target.CustomerId != request.CustomerId)
            return Result.Failure<CustomerAddressDto>("Address does not belong to the current customer.");

        if (target.IsDefault)
            return Result.Success(new CustomerAddressDto(
                target.Id, target.Label, target.Division, target.District,
                target.AreaThana, target.StreetAddress, target.ApartmentFloor, target.IsDefault));

        // Unset the current default, then promote the target.
        var currentDefault = await repository.GetDefaultAsync(request.CustomerId, cancellationToken);
        if (currentDefault is not null)
        {
            currentDefault.UnsetDefault();
            repository.Update(currentDefault);
        }

        target.SetDefault();
        repository.Update(target);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CustomerAddressDto(
            target.Id, target.Label, target.Division, target.District,
            target.AreaThana, target.StreetAddress, target.ApartmentFloor, target.IsDefault));
    }
}
