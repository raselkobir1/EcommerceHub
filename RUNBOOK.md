# EcommerceHub — Runbook

Step-by-step instructions to get the application running locally from a clean checkout.

---

## Prerequisites

| Tool | Minimum Version | Install / verify |
|---|---|---|
| .NET SDK | 8.0 | `dotnet --version` |
| Node.js | 20 LTS | `node --version` |
| Docker Desktop | 4.x | Runs PostgreSQL + Redis |
| EF Core CLI | 8.x | `dotnet tool install -g dotnet-ef` |
| PowerShell | 7.x (pwsh) | Required by `scripts/migrate.ps1` |

---

## 1. Clone the repository

```powershell
git clone <repo-url>
cd EcommerceHub
```

---

## 2. Start PostgreSQL with Docker

Run a single PostgreSQL container (no Compose file required):

```powershell
docker run -d `
  --name ecommercehub-postgres `
  -e POSTGRES_USER=postgres `
  -e POSTGRES_PASSWORD=postgres `
  -e POSTGRES_DB=ecommercehub_dev `
  -p 5432:5432 `
  postgres:16
```

If you prefer Docker Compose (includes Redis and pgAdmin), use:

```powershell
docker compose up -d
```

Verify the container is healthy:

```powershell
docker ps --filter "name=ecommercehub"
```

---

## 3. Configure the connection string

Copy the example settings file and edit it:

```powershell
Copy-Item src\EcommerceHub.API\appsettings.Development.json.example `
          src\EcommerceHub.API\appsettings.Development.json
```

Minimum keys to set in `src\EcommerceHub.API\appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ecommercehub_dev;Username=postgres;Password=postgres;",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Key": "<at-least-32-character-secret>",
    "Issuer": "EcommerceHub",
    "Audience": "EcommerceHubClients",
    "ExpiryMinutes": 60
  }
}
```

---

## 4. Run database migrations

Add initial migrations for all 10 module DbContexts (only needed once on a clean checkout):

```powershell
pwsh scripts/migrate.ps1 -Action add -Name InitialCreate
```

Apply all pending migrations to the database:

```powershell
pwsh scripts/migrate.ps1 -Action update
```

Target a single module (optional):

```powershell
pwsh scripts/migrate.ps1 -Action add -Name AddRefreshToken -Module Auth
pwsh scripts/migrate.ps1 -Action update -Module Auth
```

List migrations for a module:

```powershell
pwsh scripts/migrate.ps1 -Action list -Module Payments
```

The script covers all 10 module contexts in order:
`AuthDbContext`, `CatalogDbContext`, `OrdersDbContext`, `CartDbContext`,
`InventoryDbContext`, `PromotionsDbContext`, `CustomersDbContext`,
`SuppliersDbContext`, `ReviewsDbContext`, `PaymentsDbContext`.

---

## 5. Run the API

```powershell
cd src/EcommerceHub.API && dotnet run
```

On first run in Development mode the `DatabaseSeeder` fires automatically and:
- Creates a **SuperAdmin** user: `admin@ecommercehub.com` / `Admin@1234`
- Seeds 5 root product categories

| Endpoint | URL |
|---|---|
| Swagger UI | http://localhost:5000/swagger |
| Health check | http://localhost:5000/health |
| Hangfire dashboard | http://localhost:5000/hangfire |

---

## 6. Run the admin portal

```powershell
cd frontend/admin && npm install && npm run dev
```

Default URL: http://localhost:5173

---

## 7. Run the customer portal

```powershell
cd frontend/customer && npm install && npm run dev
```

Default URL: http://localhost:3000

---

## 8. Default credentials

| Role | Email | Password |
|---|---|---|
| SuperAdmin | admin@ecommercehub.com | Admin@1234 |

To authenticate via Swagger:

1. Open http://localhost:5000/swagger
2. Call `POST /api/auth/admin/login`:
   ```json
   { "email": "admin@ecommercehub.com", "password": "Admin@1234" }
   ```
3. Copy the returned JWT token, click **Authorize**, and paste `Bearer <token>`.
4. Exercise any protected endpoint to confirm auth is working.

---

## Day-to-day developer tasks

### Add a migration after a domain model change

```powershell
# Replace MyMigrationName with a descriptive PascalCase name
pwsh scripts/migrate.ps1 -Action add -Name MyMigrationName
pwsh scripts/migrate.ps1 -Action update
```

### Rebuild the entire solution

```powershell
dotnet build EcommerceHub.sln
```

### Run all tests

```powershell
dotnet test EcommerceHub.sln --logger "console;verbosity=normal"
```

### Tear down Docker volumes (wipes all data)

```powershell
docker compose down -v
```

---

## Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| `Connection refused` on API startup | Docker container not running | `docker compose up -d` or re-run the `docker run` command above |
| `Invalid JWT Key` exception | `Jwt:Key` missing or shorter than 32 chars | Set a longer key in `appsettings.Development.json` |
| Migration fails with `Host not found` | Wrong connection string | Check `DefaultConnection` in appsettings |
| Seed creates duplicate data on every restart | `AnyAsync()` guard failing | Verify migrations were applied; check EF schema history |
| Hangfire dashboard returns 403 | Not authenticated as SuperAdmin | Log in first, then navigate to `/hangfire` |
| `dotnet ef` command not found | EF Core CLI not installed | `dotnet tool install -g dotnet-ef` |
| Port 5432 already in use | Another PostgreSQL instance running | Stop it or change the host port in the `docker run` command |

---

## Project structure

```
EcommerceHub/
  src/
    EcommerceHub.API/            # ASP.NET Core 8 host
      Infrastructure/
        Seed/DatabaseSeeder.cs   # Auto-seeds on Development startup
    Modules/
      Auth/                      # JWT auth, admin & customer users
      Catalog/                   # Products, categories, brands
      Orders/                    # Order lifecycle
      Cart/                      # Shopping cart (Redis-backed)
      Inventory/                 # Stock management
      Promotions/                # Coupons, discounts
      Customers/                 # Customer profiles & addresses
      Suppliers/                 # Supplier management
      Reviews/                   # Product reviews & ratings
      Payments/                  # bKash, Nagad, SSLCommerz
      Notifications/             # Email / SMS / push
      Reports/                   # Analytics & reporting
    EcommerceHub.Shared.Kernel/  # Base entities, behaviours, abstractions
  frontend/
    admin/                       # Admin portal (Vite + React)
    customer/                    # Customer storefront
  scripts/
    migrate.ps1                  # EF Core migration runner (all 10 contexts)
  docker-compose.yml             # PostgreSQL + Redis + pgAdmin
  EcommerceHub.sln
```
