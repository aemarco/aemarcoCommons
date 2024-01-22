using aemarcoCommons.ToolboxTopService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ToolboxTopService;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var app = Host.CreateDefaultBuilder(args);

        app.ConfigureServices(x =>
        {
            x.AddHostedService<SampleBackgroundService>();
            x.AddWindowsService();
        });

        var exitcode = await app.RunAsTopService(x =>
        {
            x.SetServiceName("MyService");
            x.SetDisplayName("MyService");
            x.SetDescription("MyService is installed with TopService");

        });

    }
}
