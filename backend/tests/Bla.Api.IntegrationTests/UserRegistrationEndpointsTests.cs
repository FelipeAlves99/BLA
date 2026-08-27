using System.Net;
using System.Net.Http.Json;
using Shouldly;

namespace Bla.Api.IntegrationTests;

[Collection(nameof(ApiIntegrationCollection))]
public sealed class UserRegistrationEndpointsTests(PostgreSqlFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => fixture.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task PostUsers_ValidRequest_ReturnsCreated()
    {
        // Arrange
        using var client = fixture.Factory.CreateClient();
        // Act
        var response = await client.PostAsJsonAsync("/v1/users", new { username = "new.user", email = "new@bla.local", displayName = "New User", password = "a-strong-password" });
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Theory]
    [InlineData("existing", HttpStatusCode.Conflict)]
    [InlineData("failure", HttpStatusCode.BadGateway)]
    public async Task PostUsers_IdentityProviderDoesNotCreateUser_ReturnsExpectedStatus(string username, HttpStatusCode expectedStatus)
    {
        // Arrange
        using var client = fixture.Factory.CreateClient();
        // Act
        var response = await client.PostAsJsonAsync("/v1/users", new { username, email = "user@bla.local", displayName = "User", password = "a-strong-password" });
        // Assert
        response.StatusCode.ShouldBe(expectedStatus);
    }

    [Fact]
    public async Task PostUsers_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        using var client = fixture.Factory.CreateClient();
        // Act
        var response = await client.PostAsJsonAsync("/v1/users", new { username = "x", email = "invalid", displayName = "", password = "short" });
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
