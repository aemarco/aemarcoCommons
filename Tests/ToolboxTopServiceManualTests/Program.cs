using aemarcoCommons.ToolboxTopService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToolboxTopServiceManualTests;


HostApplicationBuilder app = Host.CreateApplicationBuilder(args);
app.Services.AddHostedService<SampleBackgroundService>();
_ = await app.RunAsTopService(x => x
    .SetServiceName("SomeService")
    .SetDisplayName("SomeService")
    .SetDescription("SomeService is installed with TopService"));