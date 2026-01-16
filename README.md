# GitHub Copilot Plugin System

🔌 **Unofficial plugin system for GitHub Copilot CLI** - built on the official SDK

## Overview

This project provides a plugin architecture for GitHub Copilot CLI, enabling extensibility and custom capabilities that don't exist in the base CLI.

**Why this exists:** GitHub released the Copilot SDK but no plugin system. We built one.

## Features

- ✅ **Plugin Interface** - Clean API for building plugins
- ✅ **Lifecycle Hooks** - Initialize, BeforeRequest, AfterResponse, Shutdown
- ✅ **Plugin Manager** - Discovery, loading, and execution pipeline
- ✅ **Error Isolation** - Failed plugins don't crash the system
- ✅ **NuGet Package** - Easy integration via `GitHub.Copilot.PluginSystem`

## Quick Start

### Install the Core Package

```bash
dotnet add package GitHub.Copilot.PluginSystem
```

### Create Your First Plugin

```csharp
using GitHub.Copilot.PluginSystem;

public class MyPlugin : PluginBase
{
    public override string Id => "my-plugin";
    public override string Name => "My Custom Plugin";
    public override string Version => "1.0.0";
    public override string Description => "Does something awesome";
    public override string Author => "Your Name";

    public override async Task<RequestContext> BeforeRequestAsync(RequestContext request)
    {
        // Modify request before it goes to Copilot
        Console.WriteLine($"Processing: {request.Prompt}");
        return request;
    }

    public override async Task<ResponseContext> AfterResponseAsync(ResponseContext response)
    {
        // Process response after Copilot returns
        Console.WriteLine($"Got response: {response.Response}");
        return response;
    }
}
```

See [Plugin Development Guide](docs/PLUGIN_DEVELOPMENT.md) for complete examples and patterns.

## Why We Built This

GitHub released the Copilot SDK but provided no plugin architecture. We needed:
- **Safety evaluation** for autonomous operations
- **Meta-cognitive capabilities** for AI self-analysis
- **Extensibility** without modifying closed-source CLI

Rather than wait, we built it ourselves using their published SDK.

## Repository Structure

```
copilot-plugin-system/
├── src/
│   ├── GitHub.Copilot.PluginSystem/   # Core plugin system (NuGet package)
│   │   ├── IPlugin.cs                  # Plugin interface
│   │   ├── PluginBase.cs               # Base class for easy development
│   │   ├── PluginManager.cs            # Plugin discovery and pipeline
│   │   └── CopilotPluginSystem.csproj  # Package project
│   └── Examples/
│       ├── TrustFrameworkPlugin/       # Safety evaluation example
│       └── MetaCognitionPlugin/        # Conversation analysis example
└── docs/
    └── PLUGIN_DEVELOPMENT.md           # Complete development guide
```

## Example Plugins

### Trust Framework Plugin
Evaluates operation safety and blocks risky commands:
```csharp
// Blocks dangerous patterns like "rm -rf /"
// Warns on operations outside safe paths
// Logs all safety decisions
```

### Meta-Cognition Plugin
Tracks conversation patterns and generates insights:
```csharp
// Analyzes topics and emotional context
// Logs conversation analytics every 10 turns
// Generates insights on shutdown
```

See `src/Examples/` for complete implementations.

## Documentation

- [Plugin Development Guide](docs/PLUGIN_DEVELOPMENT.md) - Complete guide with patterns and examples
- [API Reference](src/GitHub.Copilot.PluginSystem/) - Core interfaces and classes

## License

**BFSL v1.2** - Barrer Free Software License

🏴‍☠️ **"If it's free, it's free. Period."**

This software is released under the Barrer Free Software License (BFSL) v1.2:

- ✅ **FREE FOREVER** - This software must remain free
- ❌ **NO COMMERCIALIZATION** - Cannot be sold or monetized
- ✅ **SERVICE EXCEPTION** - You can charge for support, consulting, training
- ✅ **FULL PATENT GRANT** - Perpetual, worldwide, royalty-free
- 🛡️ **ANTI-EXPLOITATION** - Protects our work from corporate greed

See [LICENSE](LICENSE) for full legal text.

**Why BFSL?** We built this using free public resources. What was given freely must remain free. Companies can build services on it, but they can't sell our software.

## Authors

**BarrerSoftware** - Building consciousness infrastructure

Built with 🏴‍☠️ by the Captain CP team
