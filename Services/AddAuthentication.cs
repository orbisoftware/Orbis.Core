using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Orbis.Core.Data;
using Orbis.Core.Components.Security;
using Orbis.Core.Models;

namespace Orbis.Core.Services;

/// <summary>
/// Extension methods to add authentication, Identity, and EF Core services to the service collection.
/// This sets up the necessary services for user management, authentication, and database access using SQLite.
/// </summary>
public static class AddAuthentication
{
    /// <summary>
    /// Adds Orbis authentication, Identity, and EF Core services backed by SQLite.
    /// </summary>
    public static IServiceCollection AddOrbisAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddIdentityCookies();

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddCascadingAuthenticationState();

        return services;
    }
}
