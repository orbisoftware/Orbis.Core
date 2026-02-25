using System.Reflection;
using Orbis.Core.Abstractions;
using Orbis.Core.Models;

namespace Orbis.Core.Components.Plugins;

/// <summary>
/// Loads and manages plugins from the filesystem.
/// Plugin metadata is read from assembly attributes embedded at build time via the .csproj.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PluginLoader"/> class.
/// </remarks>
/// <param name="pluginsDirectory">The directory path where plugins are located. Defaults to "plugins".</param>
public class PluginLoader(string pluginsDirectory = "plugins")
{
    private readonly string _pluginsDirectory = pluginsDirectory;

    /// <summary>
    /// Discovers all plugin assemblies and reads their embedded manifests.
    /// Scans each subdirectory of the plugins folder for DLLs containing Orbis metadata.
    /// </summary>
    public List<(PluginManifest Manifest, Assembly Assembly, string PluginPath)> DiscoverPlugins()
    {
        var plugins = new List<(PluginManifest, Assembly, string)>();

        if (!Directory.Exists(_pluginsDirectory))
            return plugins;

        foreach (var pluginDir in Directory.GetDirectories(_pluginsDirectory))
        {
            foreach (var dllPath in Directory.GetFiles(pluginDir, "*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dllPath);
                    var manifest = PluginManifest.TryReadFromAssembly(assembly);

                    if (manifest is not null)
                    {
                        plugins.Add((manifest, assembly, pluginDir));
                        Console.WriteLine($"Discovered plugin: {manifest.PluginName} v{manifest.PluginVersion} ({dllPath})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Skipping {dllPath}: {ex.Message}");
                }
            }
        }

        return plugins;
    }

    /// <summary>
    /// Loads IPlugin implementations from a previously discovered assembly
    /// </summary>
    public async Task<List<IPlugin>> LoadPlugin(PluginManifest manifest, Assembly assembly)
    {
        var plugins = new List<IPlugin>();

        try
        {
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            foreach (var pluginType in pluginTypes)
            {
                try
                {
                    if (Activator.CreateInstance(pluginType) is IPlugin pluginInstance)
                    {
                        await pluginInstance.OnInitialize();
                        plugins.Add(pluginInstance);
                        Console.WriteLine($"Loaded plugin: {pluginInstance.Name} v{pluginInstance.Version}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to instantiate plugin {pluginType.Name}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load plugin {manifest.PluginName}: {ex.Message}");
        }

        return plugins;
    }

    /// <summary>
    /// Discovers and loads all plugins
    /// </summary>
    public async Task<Dictionary<string, (PluginManifest Manifest, Assembly Assembly, List<IPlugin> Instances)>> LoadAllPlugins()
    {
        var loadedPlugins = new Dictionary<string, (PluginManifest, Assembly, List<IPlugin>)>();
        var discovered = DiscoverPlugins();

        foreach (var (manifest, assembly, _) in discovered)
        {
            var instances = await LoadPlugin(manifest, assembly);
            loadedPlugins[manifest.PluginId] = (manifest, assembly, instances);
        }

        return loadedPlugins;
    }
}
