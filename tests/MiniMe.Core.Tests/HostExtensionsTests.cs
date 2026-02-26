using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MiniMe.Core.Agents;
using MiniMe.Core.Hosting;

namespace MiniMe.Core.Tests;

public class HostExtensionsTests
{
    [Fact]
    public void UseAgentFramework_PopulatesRegistry()
    {
        var config = new ConfigurationBuilder().Build();

        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddConfiguration(config);
        builder.Services.AddAgentFramework(builder.Configuration);
        builder.Services.AddAgent<TestAgent>();

        var host = builder.Build();
        host.UseAgentFramework();

        var registry = host.Services.GetRequiredService<AgentRegistry>();
        Assert.True(registry.TryGetAgent("TestHost", out var agent));
        Assert.NotNull(agent);
    }

    private class TestAgent : IAgent
    {
        public string Name => "TestHost";
        public Task ExecuteAsync(AgentContext context, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
