namespace GitHub.Copilot.PluginSystem;

/// <summary>
/// Core interface for all Copilot plugins
/// </summary>
public interface IPlugin
{
    string Id { get; }
    string Name { get; }
    string Version { get; }
    string Description { get; }
    string Author { get; }

    Task InitializeAsync(IPluginContext context);
    Task<RequestContext> BeforeRequestAsync(RequestContext request);
    Task<ResponseContext> AfterResponseAsync(ResponseContext response);
    Task ShutdownAsync();
}

public interface IPluginContext
{
    IDictionary<string, string> Configuration { get; }
    IDictionary<string, object> SharedData { get; }
    ILogger Logger { get; }
    CaptainCP.CopilotBridge.CopilotBridge Bridge { get; }
}

public class RequestContext
{
    public string Prompt { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public bool Cancel { get; set; }
    public string? CancelReason { get; set; }
}

public class ResponseContext
{
    public string Response { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; } = true;
    public string? Error { get; set; }
}

public interface ILogger
{
    void Debug(string message);
    void Info(string message);
    void Warning(string message);
    void Error(string message, Exception? ex = null);
}
