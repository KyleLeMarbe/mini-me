namespace MiniMe.Core.Agents;

/// <summary>
/// Provides execution context to an agent, including credentials and custom settings.
/// </summary>
public class AgentContext
{
    /// <summary>
    /// Gets the agent configuration for this execution.
    /// </summary>
    public required AgentConfiguration Configuration { get; init; }

    /// <summary>
    /// Gets the credentials associated with this agent.
    /// </summary>
    public IReadOnlyDictionary<string, string> Credentials => Configuration.Credentials;

    /// <summary>
    /// Gets custom settings for this agent.
    /// </summary>
    public IReadOnlyDictionary<string, string> Settings => Configuration.Settings;
}
