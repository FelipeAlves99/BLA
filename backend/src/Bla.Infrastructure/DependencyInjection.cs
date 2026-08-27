using Bla.Application.Common.Interfaces;
using Bla.Infrastructure.Identity;
using Bla.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bla.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddHttpClient<IIdentityAdministration, KeycloakIdentityAdministration>();
        return services;
    }
    public static IHealthChecksBuilder AddInfrastructureHealthChecks(this IHealthChecksBuilder builder) => builder.AddDbContextCheck<AppDbContext>("database", tags: ["ready"]);
}
