using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Reports.Infrastructure.Persistence;

/// <summary>
/// Read-only DbContext that points at the same PostgreSQL database but uses
/// separate DbContextOptions so it can be resolved independently of the
/// Orders / Catalog / Auth bounded contexts.  It registers no migrations and
/// owns no entity configurations — it surfaces raw query types via
/// <see cref="Database.SqlQueryRaw{T}"/> for cross-schema reporting SQL.
/// </summary>
public sealed class ReportsDbContext(DbContextOptions<ReportsDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // This context is query-only — no owned entity sets or migrations.
        base.OnModelCreating(builder);
    }
}
