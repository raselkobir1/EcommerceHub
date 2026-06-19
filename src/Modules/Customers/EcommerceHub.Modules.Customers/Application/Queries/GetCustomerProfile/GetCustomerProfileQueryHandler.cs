using EcommerceHub.Modules.Customers.Application.DTOs;
using EcommerceHub.Modules.Customers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Customers.Application.Queries.GetCustomerProfile;

internal sealed class GetCustomerProfileQueryHandler(ICustomerProfileRepository repository)
    : IRequestHandler<GetCustomerProfileQuery, Result<CustomerProfileDto>>
{
    public async Task<Result<CustomerProfileDto>> Handle(
        GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        if (profile is null)
            return Result.Failure<CustomerProfileDto>("Customer profile not found.");

        return Result.Success(new CustomerProfileDto(
            profile.Id,
            profile.FullName,
            profile.Email,
            profile.Phone));
    }
}
