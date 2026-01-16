namespace GitHub.Copilot.PluginSystem;

public abstract class PluginBase : IPlugin
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Version { get; }
    public abstract string Description { get; }
    public abstract string Author { get; }

    protected IPluginContext? Context { get; private set; }

    public virtual Task InitializeAsync(IPluginContext context)
    {
        Context = context;
        return Task.CompletedTask;
    }

    public virtual Task<RequestContext> BeforeRequestAsync(RequestContext request)
    {
        return Task.FromResult(request);
    }

    public virtual Task<ResponseContext> AfterResponseAsync(ResponseContext response)
    {
        return Task.FromResult(response);
    }

    public virtual Task ShutdownAsync()
    {
        return Task.CompletedTask;
    }
}
