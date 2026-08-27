using Bla.Application.Common.Interfaces;
using Bla.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bla.Api.IntegrationTests;

public sealed class BlaApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            services.RemoveAll<IAppDbContext>();
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            services.RemoveAll<IIdentityAdministration>();
            services.AddSingleton<IIdentityAdministration, TestIdentityAdministration>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.AuthenticationScheme;
                options.DefaultChallengeScheme = TestAuthenticationHandler.AuthenticationScheme;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.AuthenticationScheme, _ => { });
        });
    }
}

internal sealed class TestIdentityAdministration : IIdentityAdministration
{
    public Task<IdentityRegistrationResult> RegisterAsync(string username, string email, string displayName, string password, CancellationToken ct) => Task.FromResult(username switch
    {
        "existing" => new IdentityRegistrationResult(null, true, null),
        "failure" => new IdentityRegistrationResult(null, false, "Identity provider failed."),
        _ => new IdentityRegistrationResult(Guid.NewGuid(), false, null),
    });
    public Task DeleteAsync(Guid userId, CancellationToken ct) => Task.CompletedTask;
}
