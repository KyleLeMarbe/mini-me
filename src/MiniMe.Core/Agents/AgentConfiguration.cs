namespace MiniMe.Core.Agents;

/// <summary>
/// Configuration for an individual agent, loaded from appsettings.json.
/// </summary>
public class AgentConfiguration
{
    /// <summary>
    /// Gets or sets the unique name of the agent.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this agent is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the interval in seconds between agent executions.
    /// </summary>
    public int IntervalSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the credentials (API keys, passwords, tokens) for this agent.
    /// </summary>
    public Dictionary<string, string> Credentials { get; set; } = new();

    /// <summary>
    /// Gets or sets custom settings for this agent.
    /// </summary>
    public Dictionary<string, string> Settings { get; set; } = new();
}
