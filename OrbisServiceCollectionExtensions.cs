using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orbis.Core.Components.Account;
using Orbis.Core.Data;

namespace Orbis.Core;

/// <summary>
/// Extension methods to register all Orbis Core services.
/// Any frontend (Web, Desktop, etc.) just calls these to get everything wired up.
/// </summary>
public static class OrbisServiceCollectionExtensions
{
    /// <summary>
    /// Adds Orbis authentication, Identity, and EF Core services backed by SQLite.
    /// </summary>
    public static IServiceCollection AddOrbisAuthentication(this IServiceCollection services, string connectionString)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddIdentityCookies();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
        services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        services.AddCascadingAuthenticationState();

        return services;
    }

    /// <summary>
    /// Applies any pending EF Core migrations and ensures the database is created.
    /// Call this after building the app but before running it.
    /// </summary>
    public static void ApplyOrbisMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}
