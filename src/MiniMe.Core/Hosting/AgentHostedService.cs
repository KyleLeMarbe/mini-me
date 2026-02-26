using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniMe.Core.Agents;

namespace MiniMe.Core.Hosting;

/// <summary>
/// Background service that continuously runs all registered and enabled agents
/// on their configured intervals.
/// </summary>
public class AgentHostedService : BackgroundService
{
    private readonly AgentRegistry _registry;
    private readonly IOptions<AgentFrameworkOptions> _options;
    private readonly ILogger<AgentHostedService> _logger;

    public AgentHostedService(
        AgentRegistry registry,
        IOptions<AgentFrameworkOptions> options,
        ILogger<AgentHostedService> logger)
    {
        _registry = registry;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Agent framework started.");

        var agentConfigs = _options.Value.Agents
            .Where(a => a.Enabled)
            .ToList();

        if (agentConfigs.Count == 0)
        {
            _logger.LogWarning("No enabled agents configured. The framework will idle.");
            return;
        }

        var tasks = agentConfigs
            .Select(config => RunAgentLoopAsync(config, stoppingToken))
            .ToList();

        await Task.WhenAll(tasks);
    }

    private async Task RunAgentLoopAsync(AgentConfiguration config, CancellationToken stoppingToken)
    {
        if (!_registry.TryGetAgent(config.Name, out var agent) || agent is null)
        {
            _logger.LogWarning("Agent '{AgentName}' is configured but not registered. Skipping.", config.Name);
            return;
        }

        var context = new AgentContext { Configuration = config };
        var interval = TimeSpan.FromSeconds(config.IntervalSeconds);

        _logger.LogInformation("Starting agent '{AgentName}' with interval {Interval}s.", config.Name, config.IntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogDebug("Executing agent '{AgentName}'.", config.Name);
                await agent.ExecuteAsync(context, stoppingToken);
                _logger.LogDebug("Agent '{AgentName}' execution completed.", config.Name);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Agent '{AgentName}' encountered an error.", config.Name);
            }

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("Agent '{AgentName}' stopped.", config.Name);
    }
}
