using MiniMe.Core.Agents;

namespace MiniMe.Core.Tests;

public class AgentFrameworkOptionsTests
{
    [Fact]
    public void SectionName_IsAgentFramework()
    {
        Assert.Equal("AgentFramework", AgentFrameworkOptions.SectionName);
    }

    [Fact]
    public void Agents_DefaultsToEmptyList()
    {
        var options = new AgentFrameworkOptions();

        Assert.NotNull(options.Agents);
        Assert.Empty(options.Agents);
    }

    [Fact]
    public void Agents_CanBePopulated()
    {
        var options = new AgentFrameworkOptions
        {
            Agents = new List<AgentConfiguration>
            {
                new() { Name = "Agent1", IntervalSeconds = 10 },
                new() { Name = "Agent2", Enabled = false }
            }
        };

        Assert.Equal(2, options.Agents.Count);
        Assert.Equal("Agent1", options.Agents[0].Name);
        Assert.False(options.Agents[1].Enabled);
    }
}
