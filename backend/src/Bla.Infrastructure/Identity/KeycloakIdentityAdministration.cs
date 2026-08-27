using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Bla.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Bla.Infrastructure.Identity;

internal sealed class KeycloakIdentityAdministration(HttpClient client, IConfiguration configuration) : IIdentityAdministration
{
    public async Task<IdentityRegistrationResult> RegisterAsync(string username, string email, string displayName, string password, CancellationToken ct)
    {
        var authority = configuration["Auth:Authority"] ?? throw new InvalidOperationException("Auth authority is not configured.");
        var secret = configuration["KeycloakRegistration:ClientSecret"] ?? Environment.GetEnvironmentVariable("KEYCLOAK_REGISTRATION_CLIENT_SECRET") ?? throw new InvalidOperationException("Keycloak registration client secret is not configured.");
        var realm = new Uri(authority).Segments.Last().TrimEnd('/');
        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{authority}/protocol/openid-connect/token") { Content = new FormUrlEncodedContent([new("grant_type", "client_credentials"), new("client_id", "bla-registration-api"), new("client_secret", secret)]) };
        var tokenResponse = await client.SendAsync(tokenRequest, ct);
        tokenResponse.EnsureSuccessStatusCode();
        var token = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>(ct) ?? throw new InvalidOperationException("Keycloak did not return an access token.");
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{new Uri(authority).GetLeftPart(UriPartial.Authority)}/admin/realms/{realm}/users"))
        {
            Content = JsonContent.Create(new { username, email, firstName = displayName, enabled = true, emailVerified = false, credentials = new[] { new { type = "password", value = password, temporary = false } } })
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        var createResponse = await client.SendAsync(createRequest, ct);
        if (createResponse.StatusCode == HttpStatusCode.Conflict) return new IdentityRegistrationResult(null, true, null);
        if (!createResponse.IsSuccessStatusCode) return new IdentityRegistrationResult(null, false, "Could not create the account.");
        var location = createResponse.Headers.Location ?? throw new InvalidOperationException("Keycloak did not return the created user location.");
        var id = Guid.Parse(location.Segments.Last());
        return new IdentityRegistrationResult(id, false, null);
    }

    private sealed record TokenResponse([property: JsonPropertyName("access_token")] string AccessToken);
}
