# mini-me

An agentic framework to take over anything you do on a daily basis.

## Overview

Mini-Me is a .NET 10 console application that continuously runs registered sub-agents on configurable schedules. Each agent can connect to external systems (email, project management tools, etc.) using its own credentials and settings defined in configuration.

## Project Structure

```
src/
  MiniMe.Core/          Core library – agent interfaces, registry, scheduler, and DI extensions
  MiniMe.Host/          Console host that loads configuration and runs agents
tests/
  MiniMe.Core.Tests/    Unit tests for the core library
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run the Application

```bash
dotnet run --project src/MiniMe.Host
```

### Run Tests

```bash
dotnet test
```

## Configuration

Agents are registered in `src/MiniMe.Host/appsettings.json` under the `AgentFramework` section:

```json
{
  "AgentFramework": {
    "Agents": [
      {
        "Name": "Echo",
        "Enabled": true,
        "IntervalSeconds": 30,
        "Credentials": {
          "ApiKey": "your-api-key"
        },
        "Settings": {
          "Message": "Hello from the Mini-Me agent framework!"
        }
      }
    ]
  }
}
```

| Field             | Description                                      |
| ----------------- | ------------------------------------------------ |
| `Name`            | Must match the `IAgent.Name` of a registered agent |
| `Enabled`         | Set to `false` to disable the agent              |
| `IntervalSeconds` | Seconds between each execution cycle             |
| `Credentials`     | Key/value pairs for API keys, tokens, passwords  |
| `Settings`        | Key/value pairs for agent-specific settings      |

## Creating a New Agent

1. Create a class that implements `IAgent`:

```csharp
using MiniMe.Core.Agents;

public class MyAgent : IAgent
{
    public string Name => "MyAgent";

    public async Task ExecuteAsync(AgentContext context, CancellationToken cancellationToken = default)
    {
        var apiKey = context.Credentials["ApiKey"];
        // Do work here...
    }
}
```

2. Register it in `Program.cs`:

```csharp
builder.Services.AddAgent<MyAgent>();
```

3. Add its configuration to `appsettings.json`:

```json
{
  "AgentFramework": {
    "Agents": [
      {
        "Name": "MyAgent",
        "Enabled": true,
        "IntervalSeconds": 120,
        "Credentials": { "ApiKey": "abc123" },
        "Settings": {}
      }
    ]
  }
}
```
