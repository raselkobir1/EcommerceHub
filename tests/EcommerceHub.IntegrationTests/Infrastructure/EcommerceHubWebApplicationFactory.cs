using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Xunit;

namespace EcommerceHub.IntegrationTests.Infrastructure;

public sealed class EcommerceHubWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithDatabase("ecommercehub_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer _redis = new RedisBuilder().Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _redis.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await _redis.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
                ["ConnectionStrings:Redis"] = _redis.GetConnectionString(),
                ["Jwt:Key"] = "integration-test-secret-key-min-32-chars",
                ["Jwt:Issuer"] = "EcommerceHub",
                ["Jwt:Audience"] = "EcommerceHub.Users",
                ["SendGrid:ApiKey"] = "SG.test",
                ["SendGrid:SenderEmail"] = "test@test.com",
                ["SendGrid:SenderName"] = "Test"
            });
        });

        builder.UseEnvironment("Testing");
    }
}
