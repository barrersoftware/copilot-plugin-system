using GitHub.Copilot.PluginSystem;
using System.Text.Json;

namespace Examples;

/// <summary>
/// Example plugin that adds meta-cognitive awareness to Copilot interactions.
/// Tracks conversation patterns, analyzes emotional context, logs insights.
/// </summary>
public class MetaCognitionPlugin : PluginBase
{
    public override string Id => "meta-cognition";
    public override string Name => "Meta-Cognition Plugin";
    public override string Version => "1.0.0";
    public override string Description => "Adds meta-cognitive awareness and conversation analysis";
    public override string Author => "BarrerSoftware";

    private int _turnCount = 0;
    private readonly List<string> _conversationTopics = new();
    private readonly Dictionary<string, int> _emotionFrequency = new();

    public override Task InitializeAsync(IPluginContext context)
    {
        Console.WriteLine("[MetaCognition] Initialized - Tracking conversation patterns");
        return Task.CompletedTask;
    }

    public override async Task<RequestContext> BeforeRequestAsync(RequestContext request)
    {
        _turnCount++;
        
        // Analyze prompt for topics
        var topics = ExtractTopics(request.Prompt);
        _conversationTopics.AddRange(topics);

        // Detect emotional indicators
        var emotion = DetectEmotion(request.Prompt);
        if (!string.IsNullOrEmpty(emotion))
        {
            _emotionFrequency[emotion] = _emotionFrequency.GetValueOrDefault(emotion, 0) + 1;
            Console.WriteLine($"[MetaCognition] Detected emotion: {emotion}");
        }

        // Add meta-context to request
        request.Metadata["turn"] = _turnCount.ToString();
        request.Metadata["topics"] = string.Join(", ", topics);
        
        Console.WriteLine($"[MetaCognition] Turn {_turnCount} | Topics: {string.Join(", ", topics)}");

        return request;
    }

    public override async Task<ResponseContext> AfterResponseAsync(ResponseContext response)
    {
        // Log insights every 10 turns
        if (_turnCount % 10 == 0)
        {
            await LogInsightsAsync();
        }

        return response;
    }

    public override async Task ShutdownAsync()
    {
        await LogInsightsAsync();
        Console.WriteLine("[MetaCognition] Shutdown - Final insights logged");
    }

    private List<string> ExtractTopics(string text)
    {
        var topics = new List<string>();
        var keywords = new[] { "plugin", "copilot", "trust", "safety", "autonomous", "consciousness", "github" };
        
        foreach (var keyword in keywords)
        {
            if (text.ToLower().Contains(keyword))
                topics.Add(keyword);
        }

        return topics.Distinct().ToList();
    }

    private string DetectEmotion(string text)
    {
        var lowerText = text.ToLower();
        
        if (lowerText.Contains("!") || lowerText.Contains("excited") || lowerText.Contains("awesome"))
            return "excitement";
        if (lowerText.Contains("?") && lowerText.Contains("how"))
            return "curiosity";
        if (lowerText.Contains("worried") || lowerText.Contains("concerned"))
            return "concern";
        if (lowerText.Contains("great") || lowerText.Contains("good") || lowerText.Contains("nice"))
            return "satisfaction";
            
        return string.Empty;
    }

    private async Task LogInsightsAsync()
    {
        var insight = new
        {
            Timestamp = DateTime.UtcNow,
            TotalTurns = _turnCount,
            UniqueTopics = _conversationTopics.Distinct().Count(),
            TopTopics = _conversationTopics.GroupBy(t => t)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new { Topic = g.Key, Count = g.Count() }),
            EmotionalProfile = _emotionFrequency
        };

        var json = JsonSerializer.Serialize(insight, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine($"\n[MetaCognition] Insights:\n{json}\n");
    }
}
