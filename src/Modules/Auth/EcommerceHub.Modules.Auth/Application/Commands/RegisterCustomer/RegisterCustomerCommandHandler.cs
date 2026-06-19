using EcommerceHub.Modules.Auth.Application.DTOs;
using EcommerceHub.Modules.Auth.Domain.Entities;
using EcommerceHub.Modules.Auth.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Auth.Application.Commands.RegisterCustomer;

internal sealed class RegisterCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IAuthUnitOfWork unitOfWork)
    : IRequestHandler<RegisterCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(RegisterCustomerCommand request, CancellationToken ct)
    {
        if (await customerRepository.EmailExistsAsync(request.Email, ct))
            return Result.Failure<CustomerDto>("An account with this email already exists.");

        if (await customerRepository.PhoneExistsAsync(request.Phone, ct))
            return Result.Failure<CustomerDto>("An account with this phone number already exists.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);
        var customer = Customer.Create(request.FullName, request.Email, request.Phone, passwordHash);

        await customerRepository.AddAsync(customer, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new CustomerDto(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.Phone,
            customer.IsEmailVerified,
            customer.IsActive,
            customer.LastLoginAt,
            customer.CreatedAt));
    }
}
