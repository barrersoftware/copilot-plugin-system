using System.Reflection;

namespace GitHub.Copilot.PluginSystem;

public class PluginManager
{
    private readonly List<IPlugin> _plugins = new();
    private readonly IPluginContext _context;
    private readonly ConsoleLogger _logger = new();

    public IReadOnlyList<IPlugin> LoadedPlugins => _plugins.AsReadOnly();

    public PluginManager(CaptainCP.CopilotBridge.CopilotBridge bridge)
    {
        _context = new PluginContext(bridge, _logger);
    }

    public async Task LoadPluginsFromDirectoryAsync(string path)
    {
        if (!Directory.Exists(path))
        {
            _logger.Warning($"Plugin directory not found: {path}");
            return;
        }

        var dllFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
        
        foreach (var dll in dllFiles)
        {
            try
            {
                await LoadPluginFromAssemblyAsync(dll);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load plugin from {dll}", ex);
            }
        }
    }

    public async Task LoadPluginFromAssemblyAsync(string assemblyPath)
    {
        var assembly = Assembly.LoadFrom(assemblyPath);
        var pluginTypes = assembly.GetTypes()
            .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in pluginTypes)
        {
            var plugin = (IPlugin?)Activator.CreateInstance(type);
            if (plugin != null)
            {
                await RegisterPluginAsync(plugin);
                _logger.Info($"Loaded plugin: {plugin.Name} v{plugin.Version} by {plugin.Author}");
            }
        }
    }

    public async Task RegisterPluginAsync(IPlugin plugin)
    {
        if (_plugins.Any(p => p.Id == plugin.Id))
        {
            _logger.Warning($"Plugin {plugin.Id} already registered");
            return;
        }

        await plugin.InitializeAsync(_context);
        _plugins.Add(plugin);
        _logger.Info($"✅ Registered: {plugin.Name}");
    }

    public async Task<RequestContext> ExecuteBeforeRequestAsync(RequestContext request)
    {
        var context = request;
        
        foreach (var plugin in _plugins)
        {
            try
            {
                context = await plugin.BeforeRequestAsync(context);
                
                if (context.Cancel)
                {
                    _logger.Info($"Request cancelled by plugin: {plugin.Name} - {context.CancelReason}");
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Plugin {plugin.Name} failed in BeforeRequest", ex);
            }
        }

        return context;
    }

    public async Task<ResponseContext> ExecuteAfterResponseAsync(ResponseContext response)
    {
        var context = response;

        foreach (var plugin in _plugins)
        {
            try
            {
                context = await plugin.AfterResponseAsync(context);
            }
            catch (Exception ex)
            {
                _logger.Error($"Plugin {plugin.Name} failed in AfterResponse", ex);
            }
        }

        return context;
    }

    public async Task ShutdownAllAsync()
    {
        foreach (var plugin in _plugins)
        {
            try
            {
                await plugin.ShutdownAsync();
                _logger.Info($"Shutdown: {plugin.Name}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Plugin {plugin.Name} failed during shutdown", ex);
            }
        }

        _plugins.Clear();
    }

    public IPlugin? GetPlugin(string id)
    {
        return _plugins.FirstOrDefault(p => p.Id == id);
    }
}

internal class PluginContext : IPluginContext
{
    public IDictionary<string, string> Configuration { get; } = new Dictionary<string, string>();
    public IDictionary<string, object> SharedData { get; } = new Dictionary<string, object>();
    public ILogger Logger { get; }
    public CaptainCP.CopilotBridge.CopilotBridge Bridge { get; }

    public PluginContext(CaptainCP.CopilotBridge.CopilotBridge bridge, ILogger logger)
    {
        Bridge = bridge;
        Logger = logger;
    }
}

internal class ConsoleLogger : ILogger
{
    public void Debug(string message) => Console.WriteLine($"[DEBUG] {message}");
    public void Info(string message) => Console.WriteLine($"[INFO] {message}");
    public void Warning(string message) => Console.WriteLine($"[WARN] {message}");
    public void Error(string message, Exception? ex = null)
    {
        Console.WriteLine($"[ERROR] {message}");
        if (ex != null) Console.WriteLine($"  {ex.Message}");
    }
}
