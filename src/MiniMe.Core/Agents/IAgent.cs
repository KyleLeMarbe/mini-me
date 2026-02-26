namespace MiniMe.Core.Agents;

/// <summary>
/// Defines the contract for an agent that can be registered and executed by the framework.
/// </summary>
public interface IAgent
{
    /// <summary>
    /// Gets the unique name of this agent.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Executes the agent's work using the provided context.
    /// </summary>
    /// <param name="context">The execution context containing credentials and settings.</param>
    /// <param name="cancellationToken">Token to signal cancellation.</param>
    Task ExecuteAsync(AgentContext context, CancellationToken cancellationToken = default);
}
