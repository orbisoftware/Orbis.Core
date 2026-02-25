using System.Reflection;
using Orbis.Core.Abstractions;
using Orbis.Core.Models;

namespace Orbis.Core.Services;

/// <summary>
/// Registry for managing all loaded plugins.
/// Acts as the single source of truth — frontends only talk to this.
/// </summary>
public class PluginRegistry
{
    private readonly Dictionary<string, (PluginManifest Manifest, Assembly Assembly, List<IPlugin> Instances)> _plugins = [];
    private readonly PluginLoader _loader;

    public PluginRegistry(string pluginsDirectory = "plugins")
    {
        _loader = new PluginLoader(pluginsDirectory);
    }

    /// <summary>
    /// Discovers and loads all plugins from the plugins directory
    /// </summary>
    public async Task Initialize()
    {
        _plugins.Clear();
        var loaded = await _loader.LoadAllPlugins();
        foreach (var (key, value) in loaded)
        {
            _plugins[key] = value;
        }
        Console.WriteLine($"Plugin registry initialized with {_plugins.Count} plugin(s)");
    }

    /// <summary>
    /// Gets all assemblies loaded by the plugin system.
    /// Frontends use this to register additional routable assemblies.
    /// </summary>
    public IReadOnlyList<Assembly> GetLoadedAssemblies() =>
        _plugins.Values.Select(p => p.Assembly).ToList();

    /// <summary>
    /// Gets all loaded plugin manifests
    /// </summary>
    public IReadOnlyList<PluginManifest> GetAllManifests() =>
        _plugins.Values.Select(p => p.Manifest).ToList();

    /// <summary>
    /// Gets the manifest and instances for a plugin by ID
    /// </summary>
    public (PluginManifest Manifest, Assembly Assembly, List<IPlugin> Instances)? GetPlugin(string pluginId)
    {
        return _plugins.TryGetValue(pluginId, out var entry) ? entry : null;
    }

    /// <summary>
    /// Gets all plugins implementing a specific interface
    /// </summary>
    public List<T> GetPluginsOfType<T>() where T : class, IPlugin
    {
        return _plugins.Values
            .SelectMany(p => p.Instances)
            .OfType<T>()
            .ToList();
    }

    /// <summary>
    /// Unloads all plugins
    /// </summary>
    public async Task Unload()
    {
        foreach (var (_, _, instances) in _plugins.Values)
        {
            foreach (var plugin in instances)
            {
                try
                {
                    await plugin.OnUnload();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error unloading plugin {plugin.Name}: {ex.Message}");
                }
            }
        }
        _plugins.Clear();
        Console.WriteLine("Plugin registry unloaded");
    }
}
