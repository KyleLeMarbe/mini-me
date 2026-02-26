using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MiniMe.Core.Agents;
using MiniMe.Core.Hosting;

namespace MiniMe.Core.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddAgentFramework_RegistersRequiredServices()
    {
        var config = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("""
            {
                "AgentFramework": {
                    "Agents": [
                        {
                            "Name": "Test",
                            "Enabled": true,
                            "IntervalSeconds": 15
                        }
                    ]
                }
            }
            """)))
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAgentFramework(config);

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<AgentRegistry>());
        Assert.NotNull(provider.GetService<IOptions<AgentFrameworkOptions>>());

        var options = provider.GetRequiredService<IOptions<AgentFrameworkOptions>>().Value;
        Assert.Single(options.Agents);
        Assert.Equal("Test", options.Agents[0].Name);
        Assert.Equal(15, options.Agents[0].IntervalSeconds);
    }

    [Fact]
    public void AddAgent_RegistersAgentAsSingleton()
    {
        var config = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAgentFramework(config);
        services.AddAgent<TestAgent>();

        var provider = services.BuildServiceProvider();
        var agents = provider.GetServices<IAgent>().ToList();

        Assert.Single(agents);
        Assert.IsType<TestAgent>(agents[0]);
    }

    private class TestAgent : IAgent
    {
        public string Name => "Test";
        public Task ExecuteAsync(AgentContext context, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
