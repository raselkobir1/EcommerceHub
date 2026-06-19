using EcommerceHub.Modules.Auth.Domain.Events;
using EcommerceHub.Modules.Customers.Domain.Entities;
using EcommerceHub.Modules.Customers.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EcommerceHub.Modules.Customers.Application.EventHandlers;

/// <summary>
/// Seeds a CustomerProfile in the Customers module whenever a new customer registers via Auth.
/// The Auth module raises CustomerRegisteredEvent; MediatR dispatches it to all handlers
/// in the same process, including this one.
/// FullName and Phone are sourced from the RegisterCustomer command payload embedded in the event.
/// </summary>
internal sealed class CustomerRegisteredEventHandler(
    ICustomerProfileRepository profileRepository,
    ICustomersUnitOfWork unitOfWork,
    ILogger<CustomerRegisteredEventHandler> logger)
    : INotificationHandler<CustomerRegisteredEvent>
{
    public async Task Handle(CustomerRegisteredEvent notification, CancellationToken cancellationToken)
    {
        // Guard against duplicate seed (idempotent).
        var existing = await profileRepository.GetByCustomerIdAsync(notification.CustomerId, cancellationToken);
        if (existing is not null)
        {
            logger.LogWarning("CustomerProfile for {CustomerId} already exists — skipping seed.", notification.CustomerId);
            return;
        }

        var profile = CustomerProfile.Create(
            id: notification.CustomerId,
            fullName: notification.FullName,
            email: notification.Email,
            phone: notification.Phone);

        await profileRepository.AddAsync(profile, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded CustomerProfile for {CustomerId}.", notification.CustomerId);
    }
}
