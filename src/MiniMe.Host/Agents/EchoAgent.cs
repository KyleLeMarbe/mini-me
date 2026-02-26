using Microsoft.Extensions.Logging;
using MiniMe.Core.Agents;

namespace MiniMe.Host.Agents;

/// <summary>
/// A sample agent that logs a heartbeat message on each execution.
/// Demonstrates how to implement the IAgent interface.
/// </summary>
public class EchoAgent : IAgent
{
    private readonly ILogger<EchoAgent> _logger;

    public EchoAgent(ILogger<EchoAgent> logger)
    {
        _logger = logger;
    }

    public string Name => "Echo";

    public Task ExecuteAsync(AgentContext context, CancellationToken cancellationToken = default)
    {
        var message = context.Settings.TryGetValue("Message", out var msg)
            ? msg
            : "Hello from EchoAgent!";

        _logger.LogInformation("[EchoAgent] {Message}", message);
        return Task.CompletedTask;
    }
}
