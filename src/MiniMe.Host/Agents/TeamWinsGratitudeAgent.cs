using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using MiniMe.Core.Agents;

namespace MiniMe.Host.Agents;

/// <summary>
/// Agent that queries Microsoft 365 Copilot for recent team wins
/// and sends a gratitude email to the team via Microsoft Graph Mail.
/// </summary>
/// <remarks>
/// Required credentials (in appsettings.json):
///   - TenantId: Azure AD tenant identifier.
///   - ClientId: Azure AD application (client) identifier.
///   - ClientSecret: Azure AD client secret.
/// Required settings:
///   - RecipientEmail: Email address to send the thank-you message to.
/// Optional settings:
///   - CopilotPrompt: Custom prompt used to search for team wins (default provided).
///   - TeamName: Name of the team included in the email (default: "the team").
///   - SenderUserId: The user ID or UPN whose mailbox is used to send mail.
///                   When omitted, the /me endpoint is used (delegated auth).
/// </remarks>
public class TeamWinsGratitudeAgent : IAgent
{
    private readonly ILogger<TeamWinsGratitudeAgent> _logger;

    public TeamWinsGratitudeAgent(ILogger<TeamWinsGratitudeAgent> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "TeamWinsGratitude";

    /// <inheritdoc />
    public async Task ExecuteAsync(AgentContext context, CancellationToken cancellationToken = default)
    {
        var tenantId = GetRequiredCredential(context, "TenantId");
        var clientId = GetRequiredCredential(context, "ClientId");
        var clientSecret = GetRequiredCredential(context, "ClientSecret");

        var recipientEmail = GetRequiredSetting(context, "RecipientEmail");
        var teamName = context.Settings.TryGetValue("TeamName", out var tn) ? tn : "the team";
        var copilotPrompt = context.Settings.TryGetValue("CopilotPrompt", out var cp)
            ? cp
            : "Summarize the big wins and accomplishments for my team from the past week.";

        _logger.LogInformation("[TeamWinsGratitudeAgent] Authenticating with Microsoft Graph...");

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var graphClient = new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });

        _logger.LogInformation("[TeamWinsGratitudeAgent] Searching for team wins via M365 Copilot...");

        var teamWins = await SearchTeamWinsAsync(graphClient, copilotPrompt, cancellationToken);

        if (string.IsNullOrWhiteSpace(teamWins))
        {
            _logger.LogWarning("[TeamWinsGratitudeAgent] No team wins found. Skipping email.");
            return;
        }

        _logger.LogInformation("[TeamWinsGratitudeAgent] Sending gratitude email to {Recipient}...", recipientEmail);

        await SendGratitudeEmailAsync(graphClient, recipientEmail, teamName, teamWins, context, cancellationToken);

        _logger.LogInformation("[TeamWinsGratitudeAgent] Gratitude email sent successfully.");
    }

    /// <summary>
    /// Searches for team wins by querying Microsoft 365 messages and events
    /// using the Microsoft Graph search endpoint, guided by the configured prompt.
    /// </summary>
    private async Task<string> SearchTeamWinsAsync(
        GraphServiceClient graphClient,
        string copilotPrompt,
        CancellationToken cancellationToken)
    {
        try
        {
            // Use Microsoft Graph search API to find relevant messages
            // that mention wins, accomplishments, or achievements.
            var searchRequest = new Microsoft.Graph.Search.Query.QueryPostRequestBody
            {
                Requests = new List<SearchRequest>
                {
                    new SearchRequest
                    {
                        EntityTypes = new List<EntityType?>
                        {
                            EntityType.Message
                        },
                        Query = new SearchQuery
                        {
                            QueryString = copilotPrompt
                        },
                        From = 0,
                        Size = 10
                    }
                }
            };

            var searchResponse = await graphClient.Search.Query
                .PostAsQueryPostResponseAsync(searchRequest, cancellationToken: cancellationToken);

            var hits = searchResponse?.Value?
                .SelectMany(r => r.HitsContainers ?? Enumerable.Empty<SearchHitsContainer>())
                .SelectMany(c => c.Hits ?? Enumerable.Empty<SearchHit>())
                .ToList();

            if (hits is null || hits.Count == 0)
            {
                _logger.LogInformation("[TeamWinsGratitudeAgent] Search returned no results.");
                return string.Empty;
            }

            var summaryLines = new List<string>();
            foreach (var hit in hits)
            {
                var summary = hit.Summary;
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    summaryLines.Add($"• {summary}");
                }
            }

            return summaryLines.Count > 0
                ? string.Join(Environment.NewLine, summaryLines)
                : string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TeamWinsGratitudeAgent] Error searching for team wins.");
            return string.Empty;
        }
    }

    /// <summary>
    /// Sends a gratitude email containing the discovered team wins.
    /// </summary>
    private async Task SendGratitudeEmailAsync(
        GraphServiceClient graphClient,
        string recipientEmail,
        string teamName,
        string teamWins,
        AgentContext context,
        CancellationToken cancellationToken)
    {
        var message = new Message
        {
            Subject = $"Thank You, {teamName} – Celebrating Our Recent Wins!",
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = BuildEmailBody(teamName, teamWins)
            },
            ToRecipients = new List<Recipient>
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = recipientEmail
                    }
                }
            }
        };

        if (context.Settings.TryGetValue("SenderUserId", out var senderUserId)
            && !string.IsNullOrWhiteSpace(senderUserId))
        {
            var userSendMailBody = new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = true
            };

            await graphClient.Users[senderUserId].SendMail
                .PostAsync(userSendMailBody, cancellationToken: cancellationToken);
        }
        else
        {
            var meSendMailBody = new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = true
            };

            await graphClient.Me.SendMail
                .PostAsync(meSendMailBody, cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// Builds an HTML email body that highlights the team wins.
    /// </summary>
    private static string BuildEmailBody(string teamName, string teamWins)
    {
        var htmlWins = teamWins
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => $"<li>{System.Net.WebUtility.HtmlEncode(line.TrimStart('•', ' '))}</li>");

        return $"""
            <html>
            <body>
            <h2>Great Work, {System.Net.WebUtility.HtmlEncode(teamName)}! 🎉</h2>
            <p>I wanted to take a moment to recognize some of our recent wins:</p>
            <ul>
            {string.Join(Environment.NewLine, htmlWins)}
            </ul>
            <p>Thank you for all your hard work and dedication. Keep up the amazing effort!</p>
            </body>
            </html>
            """;
    }

    private static string GetRequiredCredential(AgentContext context, string key)
    {
        if (!context.Credentials.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"TeamWinsGratitudeAgent requires the '{key}' credential to be configured.");
        }

        return value;
    }

    private static string GetRequiredSetting(AgentContext context, string key)
    {
        if (!context.Settings.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"TeamWinsGratitudeAgent requires the '{key}' setting to be configured.");
        }

        return value;
    }
}
