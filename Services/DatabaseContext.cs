using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orbis.Core.Components;
using Orbis.Core.Data;
using Orbis.Core.Models;

namespace Orbis.Core.Services;

/// <summary>
/// Extension methods to add database context services and apply migrations.
/// This sets up the necessary services for database access and ensures that the database schema is up to date with the latest migrations.
/// </summary>
public static class DatabaseContext
{
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

    /// <summary>
    /// Adds the ApplicationDbContext to the service collection with a SQLite provider.
    /// This method should be called in the service registration phase to set up the database context for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to which the database context will be added.</param>
    /// <param name="connectionString">The connection string for the SQLite database.</param>
    /// <returns>The updated service collection with the database context registered.</returns>
    /// <remarks>
    /// The connection string should point to a valid SQLite database file. If the file does not exist, it will be created when the application runs and applies migrations.
    /// Example connection string: "Data Source=orbis.db"
    /// This method should be called in the ConfigureServices method of the application startup to ensure that the database context is available for dependency injection throughout the application.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if the services parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the connection string is null or empty.</exception>
    public static IServiceCollection AddOrbisDbContext(this IServiceCollection services, string connectionString)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services), "Service collection cannot be null.");

        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }
}
