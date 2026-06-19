using EcommerceHub.Modules.Auth.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using MediatR;

namespace EcommerceHub.Modules.Auth.Infrastructure.Persistence;

internal sealed class AuthUnitOfWork(AuthDbContext context, IPublisher publisher) : BaseUnitOfWork<AuthDbContext>(context, publisher), IAuthUnitOfWork;
