namespace MiniMe.Core.Agents;

/// <summary>
/// Top-level configuration options for the agent framework, bound from appsettings.json.
/// </summary>
public class AgentFrameworkOptions
{
    /// <summary>
    /// The configuration section name used in appsettings.json.
    /// </summary>
    public const string SectionName = "AgentFramework";

    /// <summary>
    /// Gets or sets the list of agent configurations.
    /// </summary>
    public List<AgentConfiguration> Agents { get; set; } = new();
}
