using System.Reflection;

namespace Orbis.Core.Models;

/// <summary>
/// Represents plugin metadata embedded in an assembly via AssemblyMetadata attributes.
/// Metadata is baked into the DLL at build time from the .csproj properties.
/// </summary>
public class PluginManifest
{
    public required string PluginId { get; init; }
    public required string PluginName { get; init; }
    public required string PluginAuthor { get; init; }
    public required string PluginVersion { get; init; }
    public string? PluginDescription { get; init; }
    public string? PluginIcon { get; init; }
    public string? BasePath { get; init; }

    /// <summary>
    /// Tries to read plugin metadata from assembly attributes.
    /// Returns null if the assembly is not an Orbis plugin.
    /// </summary>
    public static PluginManifest? TryReadFromAssembly(Assembly assembly)
    {
        var metadata = assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Where(a => a.Key.StartsWith("Orbis."))
            .ToDictionary(a => a.Key, a => a.Value);

        if (!metadata.TryGetValue("Orbis.PluginId", out var pluginId) || string.IsNullOrEmpty(pluginId))
            return null;

        if (!metadata.TryGetValue("Orbis.PluginName", out var pluginName) || string.IsNullOrEmpty(pluginName))
            return null;

        return new PluginManifest
        {
            PluginId = pluginId,
            PluginName = pluginName,
            PluginAuthor = metadata.GetValueOrDefault("Orbis.PluginAuthor") ?? "Unknown",
            PluginVersion = metadata.GetValueOrDefault("Orbis.PluginVersion") ?? "0.0.0",
            PluginDescription = metadata.GetValueOrDefault("Orbis.PluginDescription"),
            PluginIcon = metadata.GetValueOrDefault("Orbis.PluginIcon"),
            BasePath = metadata.GetValueOrDefault("Orbis.PluginBasePath"),
        };
    }
}
