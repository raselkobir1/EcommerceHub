using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Modules.Customers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Customers.Application.Commands.UpdateCustomerProfile;

internal sealed class UpdateCustomerProfileCommandHandler(
    ICustomerProfileRepository repository,
    ICustomersUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCustomerProfileCommand, Result<CustomerProfileDto>>
{
    public async Task<Result<CustomerProfileDto>> Handle(
        UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        if (profile is null)
            return Result.Failure<CustomerProfileDto>("Customer profile not found.");

        profile.UpdateProfile(request.FullName, request.Phone);
        repository.Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CustomerProfileDto(
            profile.Id,
            profile.FullName,
            profile.Email,
            profile.Phone));
    }
}
