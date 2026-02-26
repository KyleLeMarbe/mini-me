using MiniMe.Core.Agents;

namespace MiniMe.Core.Tests;

public class AgentContextTests
{
    [Fact]
    public void Credentials_ReturnsConfigurationCredentials()
    {
        var config = new AgentConfiguration
        {
            Name = "Test",
            Credentials = new Dictionary<string, string> { ["ApiKey"] = "secret123" }
        };

        var context = new AgentContext { Configuration = config };

        Assert.Equal("secret123", context.Credentials["ApiKey"]);
    }

    [Fact]
    public void Settings_ReturnsConfigurationSettings()
    {
        var config = new AgentConfiguration
        {
            Name = "Test",
            Settings = new Dictionary<string, string> { ["Url"] = "https://example.com" }
        };

        var context = new AgentContext { Configuration = config };

        Assert.Equal("https://example.com", context.Settings["Url"]);
    }

    [Fact]
    public void Configuration_IsAccessible()
    {
        var config = new AgentConfiguration { Name = "MyAgent", IntervalSeconds = 120 };
        var context = new AgentContext { Configuration = config };

        Assert.Equal("MyAgent", context.Configuration.Name);
        Assert.Equal(120, context.Configuration.IntervalSeconds);
    }
}
