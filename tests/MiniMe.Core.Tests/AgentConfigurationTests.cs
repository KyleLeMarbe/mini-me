using MiniMe.Core.Agents;

namespace MiniMe.Core.Tests;

public class AgentConfigurationTests
{
    [Fact]
    public void Defaults_AreCorrect()
    {
        var config = new AgentConfiguration();

        Assert.Equal(string.Empty, config.Name);
        Assert.True(config.Enabled);
        Assert.Equal(60, config.IntervalSeconds);
        Assert.NotNull(config.Credentials);
        Assert.Empty(config.Credentials);
        Assert.NotNull(config.Settings);
        Assert.Empty(config.Settings);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var config = new AgentConfiguration
        {
            Name = "Mail",
            Enabled = false,
            IntervalSeconds = 300,
            Credentials = new Dictionary<string, string> { ["Token"] = "abc" },
            Settings = new Dictionary<string, string> { ["Mailbox"] = "inbox" }
        };

        Assert.Equal("Mail", config.Name);
        Assert.False(config.Enabled);
        Assert.Equal(300, config.IntervalSeconds);
        Assert.Equal("abc", config.Credentials["Token"]);
        Assert.Equal("inbox", config.Settings["Mailbox"]);
    }
}
