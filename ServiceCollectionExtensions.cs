using Orbis.Core.Services;
using Orbis.Core.Components.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Orbis.Core;

/// <summary>
/// Extension methods to register all Orbis Core services.
/// Any frontend (Web, Desktop, etc.) just calls these to get everything wired up.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all core services including authentication, database context, and plugin management.
    /// This method should be called in the service registration phase of the application startup to ensure that all necessary services are available for dependency injection throughout the application.
    /// </summary>
    /// <param name="services">The service collection to which the core services will be added.</param>
    /// <param name="connectionString">The connection string for the SQLite database used by the ApplicationDbContext.</param>
    /// <returns>The updated service collection with all core services registered.</returns>
    public static IServiceCollection AddCoreServices(this IServiceCollection services, string connectionString)
    {
        services.AddOrbisAuthentication();
        services.AddOrbisDbContext(connectionString);
        services.AddSingleton<PluginRegistry>();
        services.AddScoped<PluginLoader>();

        return services;
    }
}
