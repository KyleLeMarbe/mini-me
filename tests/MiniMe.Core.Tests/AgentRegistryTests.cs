using MiniMe.Core.Agents;

namespace MiniMe.Core.Tests;

public class AgentRegistryTests
{
    [Fact]
    public void Register_AddsAgentToRegistry()
    {
        var registry = new AgentRegistry();
        var agent = new FakeAgent("TestAgent");

        registry.Register(agent);

        Assert.True(registry.TryGetAgent("TestAgent", out var resolved));
        Assert.Same(agent, resolved);
    }

    [Fact]
    public void Register_NullAgent_ThrowsArgumentNullException()
    {
        var registry = new AgentRegistry();

        Assert.Throws<ArgumentNullException>(() => registry.Register(null!));
    }

    [Fact]
    public void Register_DuplicateName_ThrowsArgumentException()
    {
        var registry = new AgentRegistry();
        registry.Register(new FakeAgent("Dup"));

        Assert.Throws<ArgumentException>(() => registry.Register(new FakeAgent("Dup")));
    }

    [Fact]
    public void TryGetAgent_NotRegistered_ReturnsFalse()
    {
        var registry = new AgentRegistry();

        Assert.False(registry.TryGetAgent("Missing", out var agent));
        Assert.Null(agent);
    }

    [Fact]
    public void TryGetAgent_IsCaseInsensitive()
    {
        var registry = new AgentRegistry();
        registry.Register(new FakeAgent("MyAgent"));

        Assert.True(registry.TryGetAgent("myagent", out _));
        Assert.True(registry.TryGetAgent("MYAGENT", out _));
    }

    [Fact]
    public void GetAll_ReturnsAllRegisteredAgents()
    {
        var registry = new AgentRegistry();
        var agent1 = new FakeAgent("Agent1");
        var agent2 = new FakeAgent("Agent2");
        registry.Register(agent1);
        registry.Register(agent2);

        var all = registry.GetAll();

        Assert.Equal(2, all.Count);
        Assert.Contains(agent1, all);
        Assert.Contains(agent2, all);
    }

    [Fact]
    public void GetAll_EmptyRegistry_ReturnsEmptyCollection()
    {
        var registry = new AgentRegistry();

        Assert.Empty(registry.GetAll());
    }

    private class FakeAgent : IAgent
    {
        public FakeAgent(string name) => Name = name;
        public string Name { get; }
        public Task ExecuteAsync(AgentContext context, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
