using Microsoft.Extensions.Hosting;
using MiniMe.Core.Hosting;
using MiniMe.Host.Agents;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddAgentFramework(builder.Configuration);
builder.Services.AddAgent<EchoAgent>();

var host = builder.Build();
host.UseAgentFramework();

await host.RunAsync();
