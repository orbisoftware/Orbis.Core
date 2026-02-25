using System.Reflection;

namespace Orbis.Core.Models;

/// <summary>
/// Represents plugin metadata embedded in an assembly via AssemblyMetadata attributes.
/// Metadata is baked into the DLL at build time from the .csproj properties.
/// </summary>
public class PluginManifest
{
    /// <summary>
    /// Gets the unique identifier for the plugin.
    /// </summary>
    public required string PluginId { get; init; }
    /// <summary>
    /// Gets the display name of the plugin.
    /// </summary>
    public required string PluginName { get; init; }
    /// <summary>
    /// Gets the author of the plugin.
    /// </summary>
    public required string PluginAuthor { get; init; }
    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    public required string PluginVersion { get; init; }
    /// <summary>
    /// Gets the optional description of the plugin.
    /// </summary>
    public string? PluginDescription { get; init; }
    /// <summary>
    /// Gets the optional icon for the plugin.
    /// </summary>
    public string? PluginIcon { get; init; }
    /// <summary>
    /// Gets the optional base path for the plugin.
    /// </summary>
    public string? BasePath { get; init; }

    /// <summary>
    /// Tries to read plugin metadata from assembly attributes.
    /// Returns null if the assembly is not an Orbis plugin.
    /// </summary>
    public static PluginManifest? TryReadFromAssembly(Assembly assembly)
    {
        var metadata = assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Where(a => a.Key.StartsWith("Plugin."))
            .ToDictionary(a => a.Key, a => a.Value);

        if (!metadata.TryGetValue("Plugin.Id", out var pluginId) || string.IsNullOrEmpty(pluginId))
            return null;

        if (!metadata.TryGetValue("Plugin.Name", out var pluginName) || string.IsNullOrEmpty(pluginName))
            return null;

        return new PluginManifest
        {
            PluginId = pluginId,
            PluginName = pluginName,
            PluginAuthor = metadata.GetValueOrDefault("Plugin.Author") ?? "Unknown",
            PluginVersion = metadata.GetValueOrDefault("Plugin.Version") ?? "0.0.0",
            PluginDescription = metadata.GetValueOrDefault("Plugin.Description"),
            PluginIcon = metadata.GetValueOrDefault("Plugin.Icon"),
            BasePath = metadata.GetValueOrDefault("Plugin.BasePath"),
        };
    }
}
