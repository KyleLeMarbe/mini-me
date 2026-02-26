using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniMe.Core.Agents;
using MiniMe.Core.Hosting;

namespace MiniMe.Core.Tests;

public class AgentHostedServiceTests
{
    [Fact]
    public async Task ExecuteAsync_RunsEnabledAgent()
    {
        var executionCount = 0;
        var agent = new CallbackAgent("Runner", _ =>
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        });

        var registry = new AgentRegistry();
        registry.Register(agent);

        var options = Options.Create(new AgentFrameworkOptions
        {
            Agents = new List<AgentConfiguration>
            {
                new() { Name = "Runner", Enabled = true, IntervalSeconds = 1 }
            }
        });

        var logger = new LoggerFactory().CreateLogger<AgentHostedService>();
        var service = new AgentHostedService(registry, options, logger);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        await service.StartAsync(cts.Token);

        // Wait for at least one execution
        await Task.Delay(1500);
        await service.StopAsync(CancellationToken.None);

        Assert.True(executionCount >= 1, $"Expected at least 1 execution but got {executionCount}.");
    }

    [Fact]
    public async Task ExecuteAsync_SkipsDisabledAgent()
    {
        var executed = false;
        var agent = new CallbackAgent("Disabled", _ =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        var registry = new AgentRegistry();
        registry.Register(agent);

        var options = Options.Create(new AgentFrameworkOptions
        {
            Agents = new List<AgentConfiguration>
            {
                new() { Name = "Disabled", Enabled = false, IntervalSeconds = 1 }
            }
        });

        var logger = new LoggerFactory().CreateLogger<AgentHostedService>();
        var service = new AgentHostedService(registry, options, logger);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await service.StartAsync(cts.Token);
        await Task.Delay(1500);
        await service.StopAsync(CancellationToken.None);

        Assert.False(executed);
    }

    [Fact]
    public async Task ExecuteAsync_ContinuesAfterAgentError()
    {
        var callCount = 0;
        var agent = new CallbackAgent("Faulty", _ =>
        {
            Interlocked.Increment(ref callCount);
            throw new InvalidOperationException("Simulated error");
        });

        var registry = new AgentRegistry();
        registry.Register(agent);

        var options = Options.Create(new AgentFrameworkOptions
        {
            Agents = new List<AgentConfiguration>
            {
                new() { Name = "Faulty", Enabled = true, IntervalSeconds = 1 }
            }
        });

        var logger = new LoggerFactory().CreateLogger<AgentHostedService>();
        var service = new AgentHostedService(registry, options, logger);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4));
        await service.StartAsync(cts.Token);
        await Task.Delay(3000);
        await service.StopAsync(CancellationToken.None);

        Assert.True(callCount >= 2, $"Expected at least 2 calls but got {callCount}. Agent should retry after errors.");
    }

    [Fact]
    public async Task ExecuteAsync_SkipsUnregisteredAgent()
    {
        var registry = new AgentRegistry();

        var options = Options.Create(new AgentFrameworkOptions
        {
            Agents = new List<AgentConfiguration>
            {
                new() { Name = "Ghost", Enabled = true, IntervalSeconds = 1 }
            }
        });

        var logger = new LoggerFactory().CreateLogger<AgentHostedService>();
        var service = new AgentHostedService(registry, options, logger);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await service.StartAsync(cts.Token);
        await Task.Delay(1500);
        await service.StopAsync(CancellationToken.None);

        // No exception should be thrown; the service gracefully skips unknown agents
    }

    private class CallbackAgent : IAgent
    {
        private readonly Func<CancellationToken, Task> _callback;

        public CallbackAgent(string name, Func<CancellationToken, Task> callback)
        {
            Name = name;
            _callback = callback;
        }

        public string Name { get; }

        public Task ExecuteAsync(AgentContext context, CancellationToken cancellationToken = default)
            => _callback(cancellationToken);
    }
}
