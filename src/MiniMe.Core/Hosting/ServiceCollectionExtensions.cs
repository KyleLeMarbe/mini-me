using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniMe.Core.Agents;

namespace MiniMe.Core.Hosting;

/// <summary>
/// Extension methods for registering the agent framework with the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the agent framework services to the service collection.
    /// Reads agent configuration from the "AgentFramework" section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAgentFramework(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AgentFrameworkOptions>(
            configuration.GetSection(AgentFrameworkOptions.SectionName));

        services.AddSingleton<AgentRegistry>();
        services.AddHostedService<AgentHostedService>();

        return services;
    }

    /// <summary>
    /// Registers an agent type with the framework.
    /// The agent will be instantiated as a singleton and added to the registry.
    /// </summary>
    /// <typeparam name="TAgent">The agent type to register.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAgent<TAgent>(this IServiceCollection services)
        where TAgent : class, IAgent
    {
        services.AddSingleton<TAgent>();
        services.AddSingleton<IAgent>(sp => sp.GetRequiredService<TAgent>());

        return services;
    }
}
