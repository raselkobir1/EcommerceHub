using EcommerceHub.Modules.Auth.Domain.Entities;
using EcommerceHub.Modules.Auth.Domain.Enums;
using EcommerceHub.Modules.Auth.Infrastructure.Persistence;
using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.API.Infrastructure.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();

        await SeedAdminUserAsync(scope.ServiceProvider, logger);
        await SeedCategoriesAsync(scope.ServiceProvider, logger);
    }

    private static async Task SeedAdminUserAsync(IServiceProvider sp, ILogger logger)
    {
        var db = sp.GetRequiredService<AuthDbContext>();
        await db.Database.MigrateAsync();

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
        await db.Database.MigrateAsync();

        if (await db.Categories.AnyAsync(c => c.ParentId == null))
        {
            logger.LogInformation("Root categories already exist — skipping category seed.");
            return;
        }

        // BD-relevant root categories (icon stored as imageUrl for display purposes)
        var categories = new[]
        {
            ("Electronics",     "electronics",      "📱", 1),
            ("Fashion",         "fashion",          "👗", 2),
            ("Home & Living",   "home-living",      "🏠", 3),
            ("Books & Education","books-education", "📚", 4),
            ("Sports & Outdoors","sports-outdoors", "⚽", 5),
            ("Health & Beauty", "health-beauty",    "💊", 6),
            ("Food & Grocery",  "food-grocery",     "🛒", 7),
            ("Toys & Kids",     "toys-kids",        "🧸", 8),
        };

        foreach (var (name, slug, icon, sortOrder) in categories)
            db.Categories.Add(Category.Create(name, slug, null, icon, sortOrder));

        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} root categories", categories.Length);
    }
}
