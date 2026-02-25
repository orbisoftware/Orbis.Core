namespace Orbis.Core.Abstractions;

/// <summary>
/// Base interface that all plugins must implement
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Gets the plugin name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the plugin version
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the plugin author
    /// </summary>
    string Author { get; }

    /// <summary>
    /// Called when the plugin is initialized
    /// </summary>
    Task OnInitialize();

    /// <summary>
    /// Called when the plugin is being unloaded
    /// </summary>
    Task OnUnload();
}
