using System.Net;
using System.Net.Http.Json;
using EcommerceHub.IntegrationTests.Infrastructure;
using EcommerceHub.Shared.Kernel.Common;
using FluentAssertions;
using Xunit;

namespace EcommerceHub.IntegrationTests.Auth;

public sealed class AdminAuthControllerTests(EcommerceHubWebApplicationFactory factory)
    : IClassFixture<EcommerceHubWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/admin/login", new
        {
            email = "superadmin@ecommercehub.com",
            password = "Admin@12345"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        result!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/admin/login", new
        {
            email = "superadmin@ecommercehub.com",
            password = "WrongPassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ShouldReturnUnprocessableEntity()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/admin/login", new
        {
            email = "not-an-email",
            password = "password"
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnHealthy()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
