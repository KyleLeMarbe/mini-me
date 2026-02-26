using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniMe.Core.Agents;

namespace MiniMe.Core.Hosting;

/// <summary>
/// Extension methods for populating the agent registry after the host is built.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Populates the agent registry with all registered IAgent instances.
    /// Call this after building the host and before running it.
    /// </summary>
    /// <param name="host">The built host.</param>
    /// <returns>The host for chaining.</returns>
    public static IHost UseAgentFramework(this IHost host)
    {
        var registry = host.Services.GetRequiredService<AgentRegistry>();
        var agents = host.Services.GetServices<IAgent>();

        foreach (var agent in agents)
        {
            registry.Register(agent);
        }

        return host;
    }
}
