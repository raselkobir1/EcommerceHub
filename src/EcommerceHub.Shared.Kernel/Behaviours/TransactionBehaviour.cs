using EcommerceHub.Shared.Kernel.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EcommerceHub.Shared.Kernel.Behaviours;

[AttributeUsage(AttributeTargets.Class)]
public sealed class TransactionalAttribute : Attribute { }

public sealed class TransactionBehaviour<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var isTransactional = typeof(TRequest).GetCustomAttributes(typeof(TransactionalAttribute), false).Any();
        if (!isTransactional) return await next();

        logger.LogDebug("Beginning transaction for {RequestName}", typeof(TRequest).Name);
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var response = await next();
            await unitOfWork.CommitTransactionAsync(ct);
            return response;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
