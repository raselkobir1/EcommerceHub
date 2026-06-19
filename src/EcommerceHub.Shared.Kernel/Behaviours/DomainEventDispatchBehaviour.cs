using EcommerceHub.Shared.Kernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Shared.Kernel.Behaviours;

public sealed class DomainEventDispatchBehaviour<TRequest, TResponse>(
    IMediator mediator,
    DbContext context)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = await next();

        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in events)
            await mediator.Publish(domainEvent, ct);

        return response;
    }
}
