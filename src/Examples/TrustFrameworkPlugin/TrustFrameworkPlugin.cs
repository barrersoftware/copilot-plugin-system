using GitHub.Copilot.PluginSystem;

namespace Examples;

/// <summary>
/// Example plugin that evaluates safety of operations before allowing execution.
/// Demonstrates using BeforeRequest to implement a trust/safety layer.
/// </summary>
public class TrustFrameworkPlugin : PluginBase
{
    public override string Id => "trust-framework";
    public override string Name => "Trust Framework Plugin";
    public override string Version => "1.0.0";
    public override string Description => "Evaluates operation safety and blocks risky operations";
    public override string Author => "BarrerSoftware";

    private readonly List<string> _safePaths = new()
    {
        "/tmp/", "~/.cp-state/", "~/captain-cp/"
    };

    private readonly List<string> _unsafePatterns = new()
    {
        "rm -rf /", "rm -rf ~", "dd if=", "> /dev/", "mkfs", "fdisk"
    };

    public override Task InitializeAsync(IPluginContext context)
    {
        Console.WriteLine("[TrustFramework] Initialized - Protecting system from risky operations");
        return Task.CompletedTask;
    }

    public override async Task<RequestContext> BeforeRequestAsync(RequestContext request)
    {
        var prompt = request.Prompt.ToLower();

        // Check for unsafe patterns
        foreach (var pattern in _unsafePatterns)
        {
            if (prompt.Contains(pattern.ToLower()))
            {
                Console.WriteLine($"[TrustFramework] ⛔ BLOCKED: Detected unsafe pattern '{pattern}'");
                request.Metadata["blocked"] = "true";
                request.Metadata["reason"] = $"Unsafe pattern detected: {pattern}";
                request.Prompt = "I cannot execute that operation as it appears to be destructive. Please verify your intent.";
                return request;
            }
        }

        // Validate paths if file operations detected
        if (prompt.Contains("write") || prompt.Contains("delete") || prompt.Contains("modify"))
        {
            var isSafePath = _safePaths.Any(safe => prompt.Contains(safe));
            if (!isSafePath)
            {
                Console.WriteLine("[TrustFramework] ⚠️  WARNING: File operation outside safe paths");
                request.Metadata["warning"] = "true";
                request.Prompt = $"[TRUST CHECK] {request.Prompt}\n\nNote: This operation is outside safe paths. Proceed with caution.";
            }
            else
            {
                Console.WriteLine("[TrustFramework] ✅ Safe path detected");
            }
        }

        return request;
    }

    public override async Task<ResponseContext> AfterResponseAsync(ResponseContext response)
    {
        if (response.Metadata.ContainsKey("blocked"))
        {
            Console.WriteLine($"[TrustFramework] Operation was blocked: {response.Metadata["reason"]}");
        }
        return response;
    }

    public override Task ShutdownAsync()
    {
        Console.WriteLine("[TrustFramework] Shutdown - Protected session ended");
        return Task.CompletedTask;
    }
}
