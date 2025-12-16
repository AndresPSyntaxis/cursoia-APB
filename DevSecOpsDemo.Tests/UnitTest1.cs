using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DevSecOpsDemo.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DevSecOpsDemo.Tests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<DevSecOpsDemo.Api.Program>>
{
    private readonly WebApplicationFactory<DevSecOpsDemo.Api.Program> _factory;

    public ApiIntegrationTests(WebApplicationFactory<DevSecOpsDemo.Api.Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetHealth_ReturnsOkWithStatus()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("ok", content.GetProperty("status").GetString());
    }

    [Fact]
    public async Task PostSuma_ValidInput_ReturnsSum()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { A = 5, B = 3 };

        // Act
        var response = await client.PostAsJsonAsync("/api/suma", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(8, content.GetProperty("result").GetInt32());
    }

    [Fact]
    public async Task PostSuma_InvalidInput_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { A = 5 }; // Missing B

        // Act
        var response = await client.PostAsJsonAsync("/api/suma", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Invalid request body", content.GetProperty("error").GetString());
    }

    [Fact]
    public async Task PostSuma_NullBody_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/api/suma", new StringContent("", System.Text.Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Invalid request body", content.GetProperty("error").GetString());
    }
}
