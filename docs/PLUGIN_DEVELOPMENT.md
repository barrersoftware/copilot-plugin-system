# Plugin Development Guide

## Creating Your First Plugin

### Step 1: Install the Package

```bash
dotnet new classlib -n MyPlugin
cd MyPlugin
dotnet add package GitHub.Copilot.PluginSystem
```

### Step 2: Implement the Plugin Interface

You have two options:

#### Option A: Inherit from PluginBase (Recommended)

```csharp
using GitHub.Copilot.PluginSystem;

public class MyPlugin : PluginBase
{
    public override string Id => "my-plugin";
    public override string Name => "My Plugin";
    public override string Version => "1.0.0";
    public override string Description => "Does something cool";
    public override string Author => "Your Name";

    public override async Task<RequestContext> BeforeRequestAsync(RequestContext request)
    {
        // Your logic here
        return request;
    }
}
```

#### Option B: Implement IPlugin Directly

```csharp
using GitHub.Copilot.PluginSystem;

public class MyPlugin : IPlugin
{
    public string Id => "my-plugin";
    public string Name => "My Plugin";
    // ... implement all interface members
}
```

### Step 3: Implement Lifecycle Hooks

```csharp
public override Task InitializeAsync(IPluginContext context)
{
    // Setup: Load config, connect to services, etc.
    Console.WriteLine($"[{Name}] Initialized");
    return Task.CompletedTask;
}

public override async Task<RequestContext> BeforeRequestAsync(RequestContext request)
{
    // Intercept/modify requests before they go to Copilot
    // - Add context
    // - Validate safety
    // - Log metrics
    return request;
}

public override async Task<ResponseContext> AfterResponseAsync(ResponseContext response)
{
    // Process responses after Copilot returns
    // - Extract insights
    // - Log results
    // - Trigger actions
    return response;
}

public override Task ShutdownAsync()
{
    // Cleanup: Close connections, save state, etc.
    Console.WriteLine($"[{Name}] Shutdown");
    return Task.CompletedTask;
}
```

## Plugin Patterns

### Safety/Trust Plugin

```csharp
public override async Task<RequestContext> BeforeRequestAsync(RequestContext request)
{
    if (ContainsUnsafePattern(request.Prompt))
    {
        request.Metadata["blocked"] = "true";
        request.Prompt = "Operation blocked for safety reasons";
    }
    return request;
}
```

### Logging Plugin

```csharp
public override async Task<ResponseContext> AfterResponseAsync(ResponseContext response)
{
    await File.AppendAllTextAsync("copilot-log.txt", 
        $"{DateTime.Now}: {response.Response}\n");
    return response;
}
```

### Context Enhancement Plugin

```csharp
public override async Task<RequestContext> BeforeRequestAsync(RequestContext request)
{
    var projectContext = await LoadProjectContextAsync();
    request.Prompt = $"{projectContext}\n\n{request.Prompt}";
    return request;
}
```

### Analytics Plugin

```csharp
private int _requestCount = 0;
private Dictionary<string, int> _topicCounts = new();

public override async Task<RequestContext> BeforeRequestAsync(RequestContext request)
{
    _requestCount++;
    var topics = ExtractTopics(request.Prompt);
    
    foreach (var topic in topics)
        _topicCounts[topic] = _topicCounts.GetValueOrDefault(topic, 0) + 1;
    
    return request;
}
```

## Using the Plugin Manager

```csharp
using GitHub.Copilot.PluginSystem;

var manager = new PluginManager();

// Load from directory
await manager.LoadPluginsFromDirectoryAsync("./plugins");

// Or register manually
await manager.RegisterPluginAsync(new MyPlugin());

// Execute before request pipeline
var modifiedRequest = await manager.ExecuteBeforeRequestAsync(originalRequest);

// Execute after response pipeline
var modifiedResponse = await manager.ExecuteAfterResponseAsync(originalResponse);

// Shutdown all plugins
await manager.ShutdownAllAsync();
```

## Best Practices

1. **Error Handling**: Wrap plugin logic in try/catch - plugin failures shouldn't crash the host
2. **Performance**: Keep BeforeRequest fast - it blocks the request pipeline
3. **State Management**: Use metadata dictionary for passing data between hooks
4. **Logging**: Use Console.WriteLine with plugin name prefix for debugging
5. **Testing**: Write unit tests for your plugin logic
6. **Versioning**: Follow semantic versioning for your plugins

## Metadata Usage

The `RequestContext` and `ResponseContext` include a `Metadata` dictionary for passing data:

```csharp
// In BeforeRequest
request.Metadata["analyzed"] = "true";
request.Metadata["risk_level"] = "low";

// In AfterResponse
if (response.Metadata.ContainsKey("analyzed"))
{
    var riskLevel = response.Metadata["risk_level"];
    // Handle accordingly
}
```

## Example Plugins

Check out the `src/Examples/` directory for complete working examples:

- **TrustFrameworkPlugin** - Safety evaluation and blocking
- **MetaCognitionPlugin** - Conversation analysis and insights

## Publishing Your Plugin

1. Build your plugin: `dotnet build -c Release`
2. Package it: `dotnet pack -c Release`
3. Publish to NuGet: `dotnet nuget push *.nupkg --source https://api.nuget.org/v3/index.json`
4. Share with the community!

## Need Help?

Open an issue on GitHub: https://github.com/barrersoftware/copilot-plugin-system/issues
