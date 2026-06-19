using System.Text;
using EcommerceHub.API.Infrastructure.Seed;
using EcommerceHub.API.Infrastructure.Services;
using EcommerceHub.API.Middleware;
using EcommerceHub.Modules.Auth;
using EcommerceHub.Modules.Cart;
using EcommerceHub.Modules.Catalog;
using EcommerceHub.Modules.Customers;
using EcommerceHub.Modules.Inventory;
using EcommerceHub.Modules.Notifications;
using EcommerceHub.Modules.Orders;
using EcommerceHub.Modules.Payments;
using EcommerceHub.Modules.Promotions;
using EcommerceHub.Modules.Reports;
using EcommerceHub.Modules.Reviews;
using EcommerceHub.Modules.Suppliers;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Behaviours;
using EcommerceHub.Shared.Kernel.Persistence;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File("logs/ecommercehub-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();

// ── Core services ─────────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUserService>();
builder.Services.AddScoped<AuditInterceptor>();

// ── MediatR pipeline behaviours ───────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehaviour<,>));
});

// ── Register all modules ──────────────────────────────────────────────────────
var modules = new IModule[]
{
    new AuthModule(),
    new CatalogModule(),
    new OrdersModule(),
    new CartModule(),
    new InventoryModule(),
    new PromotionsModule(),
    new CustomersModule(),
    new SuppliersModule(),
    new ReviewsModule(),
    new PaymentsModule(),
    new NotificationsModule(),
    new ReportsModule()
};

foreach (var module in modules)
    module.RegisterModule(builder.Services, builder.Configuration);

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
        opts.Events = new JwtBearerEvents
        {
            OnChallenge = ctx =>
            {
                ctx.HandleResponse();
                ctx.Response.StatusCode = 401;
                ctx.Response.ContentType = "application/json";
                return ctx.Response.WriteAsync("{\"success\":false,\"message\":\"Unauthorized. Please login.\"}");
            },
            OnForbidden = ctx =>
            {
                ctx.Response.StatusCode = 403;
                ctx.Response.ContentType = "application/json";
                return ctx.Response.WriteAsync("{\"success\":false,\"message\":\"Forbidden. Insufficient permissions.\"}");
            }
        };
    });

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("SuperAdmin", p => p.RequireRole("SuperAdmin"));
    opts.AddPolicy("Admin", p => p.RequireRole("SuperAdmin", "Admin"));
    opts.AddPolicy("Manager", p => p.RequireRole("SuperAdmin", "Admin", "Manager"));
    opts.AddPolicy("Staff", p => p.RequireRole("SuperAdmin", "Admin", "Manager", "Staff"));
    opts.AddPolicy("Customer", p => p.RequireRole("Customer"));
    opts.AddPolicy("AnyAdmin", p => p.RequireRole("SuperAdmin", "Admin", "Manager", "Staff"));
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(opts =>
{
    var adminOrigins = builder.Configuration.GetSection("Cors:AdminOrigins").Get<string[]>() ?? ["http://localhost:5173"];
    var customerOrigins = builder.Configuration.GetSection("Cors:CustomerOrigins").Get<string[]>() ?? ["http://localhost:3000"];
    var allOrigins = adminOrigins.Concat(customerOrigins).ToArray();

    opts.AddPolicy("AllPortals", p =>
        p.WithOrigins(allOrigins)
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials());

    opts.AddPolicy("AdminPortal", p =>
        p.WithOrigins(adminOrigins)
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials());

    opts.AddPolicy("CustomerPortal", p =>
        p.WithOrigins(customerOrigins)
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials());
});

// ── Hangfire ──────────────────────────────────────────────────────────────────
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddHangfireServer();

// ── Controllers & Swagger ─────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EcommerceHub API",
        Version = "v1",
        Description = "Bangladesh E-Commerce Platform — Modular Monolith"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Bearer token. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            []
        }
    });
});

// ── Health Checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgresql")
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379", name: "redis");

// ── Build App ─────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcommerceHub API v1");
        c.RoutePrefix = string.Empty;
    });
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("AllPortals");
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new EcommerceHub.API.Infrastructure.Hangfire.HangfireDashboardAuthorizationFilter()]
});

app.MapControllers();
app.MapHealthChecks("/health");

// Database seed
if (app.Environment.IsDevelopment())
{
    await DatabaseSeeder.SeedAsync(app.Services, app.Logger);
}

app.Run();
