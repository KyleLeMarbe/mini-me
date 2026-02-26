namespace MiniMe.Core.Agents;

/// <summary>
/// Registry for managing agent registrations. Agents are registered by name 
/// and can be resolved at runtime for execution.
/// </summary>
public class AgentRegistry
{
    private readonly Dictionary<string, IAgent> _agents = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers an agent in the registry.
    /// </summary>
    /// <param name="agent">The agent to register.</param>
    /// <exception cref="ArgumentException">Thrown when an agent with the same name is already registered.</exception>
    public void Register(IAgent agent)
    {
        ArgumentNullException.ThrowIfNull(agent);

        if (!_agents.TryAdd(agent.Name, agent))
        {
            throw new ArgumentException($"An agent with the name '{agent.Name}' is already registered.");
        }
    }

    /// <summary>
    /// Attempts to get a registered agent by name.
    /// </summary>
    /// <param name="name">The name of the agent.</param>
    /// <param name="agent">The resolved agent, if found.</param>
    /// <returns>True if the agent was found; otherwise, false.</returns>
    public bool TryGetAgent(string name, out IAgent? agent)
    {
        return _agents.TryGetValue(name, out agent);
    }

    /// <summary>
    /// Gets all registered agents.
    /// </summary>
    public IReadOnlyCollection<IAgent> GetAll() => _agents.Values;
}
