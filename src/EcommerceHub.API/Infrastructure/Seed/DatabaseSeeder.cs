using EcommerceHub.Modules.Auth.Domain.Entities;
using EcommerceHub.Modules.Auth.Domain.Enums;
using EcommerceHub.Modules.Auth.Infrastructure.Persistence;
using EcommerceHub.Modules.Cart.Infrastructure.Persistence;
using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Infrastructure.Persistence;
using EcommerceHub.Modules.Customers.Infrastructure.Persistence;
using EcommerceHub.Modules.Inventory.Infrastructure.Persistence;
using EcommerceHub.Modules.Orders.Infrastructure.Persistence;
using EcommerceHub.Modules.Payments.Infrastructure.Persistence;
using EcommerceHub.Modules.Promotions.Infrastructure.Persistence;
using EcommerceHub.Modules.Reviews.Infrastructure.Persistence;
using EcommerceHub.Modules.Suppliers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.API.Infrastructure.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        await MigrateAllAsync(sp, logger);
        await SeedAdminUserAsync(sp, logger);
        await SeedCategoriesAsync(sp, logger);
    }

    private static async Task MigrateAllAsync(IServiceProvider sp, ILogger logger)
    {
        var contexts = new[]
        {
            ("Auth",       (DbContext)sp.GetRequiredService<AuthDbContext>()),
            ("Catalog",    sp.GetRequiredService<CatalogDbContext>()),
            ("Orders",     sp.GetRequiredService<OrdersDbContext>()),
            ("Cart",       sp.GetRequiredService<CartDbContext>()),
            ("Inventory",  sp.GetRequiredService<InventoryDbContext>()),
            ("Promotions", sp.GetRequiredService<PromotionsDbContext>()),
            ("Customers",  sp.GetRequiredService<CustomersDbContext>()),
            ("Suppliers",  sp.GetRequiredService<SuppliersDbContext>()),
            ("Reviews",    sp.GetRequiredService<ReviewsDbContext>()),
            ("Payments",   sp.GetRequiredService<PaymentsDbContext>()),
        };

        foreach (var (name, ctx) in contexts)
        {
            await ctx.Database.MigrateAsync();
            logger.LogInformation("{Module} DB migrated.", name);
        }
    }

    private static async Task SeedAdminUserAsync(IServiceProvider sp, ILogger logger)
    {
        var db = sp.GetRequiredService<AuthDbContext>();

        const string adminEmail = "admin@ecommercehub.com";

        if (await db.AdminUsers.AnyAsync(u => u.Email == adminEmail))
        {
            logger.LogInformation("SuperAdmin already exists — skipping admin seed.");
            return;
        }

        var admin = AdminUser.Create(
            fullName: "System Admin",
            email: adminEmail,
            passwordHash: BCrypt.Net.BCrypt.HashPassword("Admin@1234", 12),
            role: AdminRole.SuperAdmin);

        db.AdminUsers.Add(admin);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded default SuperAdmin: {Email} / Admin@1234", adminEmail);
    }

    private static async Task SeedCategoriesAsync(IServiceProvider sp, ILogger logger)
    {
        var db = sp.GetRequiredService<CatalogDbContext>();

        if (await db.Categories.AnyAsync(c => c.ParentId == null))
        {
            logger.LogInformation("Root categories already exist — skipping category seed.");
            return;
        }

        var categories = new[]
        {
            ("Electronics",      "electronics",       "📱", 1),
            ("Fashion",          "fashion",           "👗", 2),
            ("Home & Living",    "home-living",       "🏠", 3),
            ("Books & Education","books-education",   "📚", 4),
            ("Sports & Outdoors","sports-outdoors",   "⚽", 5),
            ("Health & Beauty",  "health-beauty",     "💊", 6),
            ("Food & Grocery",   "food-grocery",      "🛒", 7),
            ("Toys & Kids",      "toys-kids",         "🧸", 8),
        };

        foreach (var (name, slug, icon, sortOrder) in categories)
            db.Categories.Add(Category.Create(name, slug, null, icon, sortOrder));

        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} root categories", categories.Length);
    }
}
