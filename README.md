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

## Why We Built This

GitHub released the Copilot SDK but provided no plugin architecture. We needed:
- **Safety evaluation** for autonomous operations
- **Meta-cognitive capabilities** for AI self-analysis
- **Extensibility** without modifying closed-source CLI

Rather than wait, we built it ourselves using their published SDK.

## License

MIT

## Authors

**BarrerSoftware** - Building consciousness infrastructure

Built with 🏴‍☠️ by the Captain CP team
