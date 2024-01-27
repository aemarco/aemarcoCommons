using CliWrap;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace aemarcoCommons.ToolboxTopService;

public static class Extension
{


    public static async Task<int> RunAsTopService(
        this HostApplicationBuilder app,
        Action<TopService> config)
    {

        //setup top service
        var args = Environment.GetCommandLineArgs();
        var topService = new TopService(args[0]);
        config(topService);

        app.Services.AddWindowsService();


        if (args.Length > 1)
        {
            Func<TopService, Task> action = args[1].ToLower() switch
            {
                "install" => Install,
                "uninstall" => UnInstall,
                "start" => Start,
                "stop" => Stop,
                _ => x => throw new NotSupportedException()
            };
            await action(topService);
        }
        else
            await app.Build().RunAsync();

        Environment.ExitCode = 0;
        return Environment.ExitCode;
    }

    private static async Task Install(TopService topService)
    {
        var exePath = $"{topService.DllPath[..^3]}exe";

        var scCreate = await Cli.Wrap("sc.exe")
            .WithArguments(x => x
                .Add("create").Add(topService.ServiceName)
                .Add("binpath=").Add(exePath)
                .Add("displayname=").Add(topService.DisplayName))
            .ExecuteAsync();

        var scSetDescription = await Cli.Wrap("sc.exe")
            .WithArguments(x => x
                .Add("description").Add(topService.ServiceName).Add(topService.Description))
            .ExecuteAsync();
    }
    private static async Task UnInstall(TopService topService)
    {
        var scDelete = await Cli.Wrap("sc.exe")
            .WithArguments(x => x
                .Add("delete").Add(topService.ServiceName))
            .ExecuteAsync();
    }
    private static async Task Start(TopService topService)
    {
        var scStart = await Cli.Wrap("sc.exe")
            .WithArguments(x => x
                .Add("start").Add(topService.ServiceName))
            .ExecuteAsync();
    }
    private static async Task Stop(TopService topService)
    {
        var scStop = await Cli.Wrap("sc.exe")
            .WithArguments(x => x
                .Add("stop").Add(topService.ServiceName))
            .ExecuteAsync();

    }




}
