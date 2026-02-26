using Microsoft.Extensions.Logging;
using MiniMe.Core.Agents;
using MiniMe.Host.Agents;

namespace MiniMe.Host.Tests;

public class TeamWinsGratitudeAgentTests
{
    [Fact]
    public void Name_ReturnsExpectedValue()
    {
        var logger = new LoggerFactory().CreateLogger<TeamWinsGratitudeAgent>();
        var agent = new TeamWinsGratitudeAgent(logger);

        Assert.Equal("TeamWinsGratitude", agent.Name);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsWhenTenantIdMissing()
    {
        var logger = new LoggerFactory().CreateLogger<TeamWinsGratitudeAgent>();
        var agent = new TeamWinsGratitudeAgent(logger);

        var context = new AgentContext
        {
            Configuration = new AgentConfiguration
            {
                Name = "TeamWinsGratitude",
                Credentials = new Dictionary<string, string>(),
                Settings = new Dictionary<string, string>
                {
                    ["RecipientEmail"] = "team@example.com"
                }
            }
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => agent.ExecuteAsync(context));
        Assert.Contains("TenantId", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsWhenClientIdMissing()
    {
        var logger = new LoggerFactory().CreateLogger<TeamWinsGratitudeAgent>();
        var agent = new TeamWinsGratitudeAgent(logger);

        var context = new AgentContext
        {
            Configuration = new AgentConfiguration
            {
                Name = "TeamWinsGratitude",
                Credentials = new Dictionary<string, string>
                {
                    ["TenantId"] = "tenant-id"
                },
                Settings = new Dictionary<string, string>
                {
                    ["RecipientEmail"] = "team@example.com"
                }
            }
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => agent.ExecuteAsync(context));
        Assert.Contains("ClientId", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsWhenClientSecretMissing()
    {
        var logger = new LoggerFactory().CreateLogger<TeamWinsGratitudeAgent>();
        var agent = new TeamWinsGratitudeAgent(logger);

        var context = new AgentContext
        {
            Configuration = new AgentConfiguration
            {
                Name = "TeamWinsGratitude",
                Credentials = new Dictionary<string, string>
                {
                    ["TenantId"] = "tenant-id",
                    ["ClientId"] = "client-id"
                },
                Settings = new Dictionary<string, string>
                {
                    ["RecipientEmail"] = "team@example.com"
                }
            }
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => agent.ExecuteAsync(context));
        Assert.Contains("ClientSecret", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsWhenRecipientEmailMissing()
    {
        var logger = new LoggerFactory().CreateLogger<TeamWinsGratitudeAgent>();
        var agent = new TeamWinsGratitudeAgent(logger);

        var context = new AgentContext
        {
            Configuration = new AgentConfiguration
            {
                Name = "TeamWinsGratitude",
                Credentials = new Dictionary<string, string>
                {
                    ["TenantId"] = "tenant-id",
                    ["ClientId"] = "client-id",
                    ["ClientSecret"] = "client-secret"
                },
                Settings = new Dictionary<string, string>()
            }
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => agent.ExecuteAsync(context));
        Assert.Contains("RecipientEmail", ex.Message);
    }

    [Fact]
    public void Agent_ImplementsIAgent()
    {
        var logger = new LoggerFactory().CreateLogger<TeamWinsGratitudeAgent>();
        var agent = new TeamWinsGratitudeAgent(logger);

        Assert.IsAssignableFrom<IAgent>(agent);
    }
}
