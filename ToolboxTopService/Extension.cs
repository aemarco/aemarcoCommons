using CliWrap;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace aemarcoCommons.ToolboxTopService;

public static class Extension
{
    public static async Task<int> RunAsTopService(this IHostBuilder hostBuilder,
        Action<TopService> config)
    {
        var service = new TopService();
        config(service);

        var args = Environment.GetCommandLineArgs();
        if (args.Length <= 1)
        {
            hostBuilder.Build().Run();
            return 0;
        }

        if (args[1].Equals("install", StringComparison.CurrentCultureIgnoreCase))
        {
            var scCreate = await Cli.Wrap("sc.exe")
                        .WithArguments(x => x
                            .Add("create")
                            .Add(service.ServiceName)
                            .Add("binpath=")
                            .Add("C:\\dev\\aemarcoCommons\\ToolboxTopService\\bin\\Debug\\net8.0-windows\\aemarcoCommons.ToolboxTopService.exe")
                            .Add("displayname=")
                            .Add(service.DisplayName)
                            ).ExecuteAsync();

            var scSetDescription = await Cli.Wrap("sc.exe")
                .WithArguments(x => x
                    .Add("description")
                    .Add(service.ServiceName)
                    .Add(service.Description)).ExecuteAsync();
        }
        else if (args[1].Equals("remove", StringComparison.CurrentCultureIgnoreCase))
        {
            var scDelete = await Cli.Wrap("sc.exe")
                       .WithArguments(x => x
                           .Add("delete")
                           .Add(service.ServiceName)).ExecuteAsync();
        }
        else if (args[1].Equals("start", StringComparison.CurrentCultureIgnoreCase))
        {
            var scDelete = await Cli.Wrap("sc.exe")
                       .WithArguments(x => x
                           .Add("start")
                           .Add(service.ServiceName)).ExecuteAsync();
        }
        else if (args[1].Equals("stop", StringComparison.CurrentCultureIgnoreCase))
        {
            var scDelete = await Cli.Wrap("sc.exe")
                       .WithArguments(x => x
                           .Add("stop")
                           .Add(service.ServiceName)).ExecuteAsync();
        }



        return 0;
    }

    public static TopService SetServiceName(this TopService service, string name)
    {
        service.ServiceName = name;
        return service;
    }

    public static TopService SetDisplayName(this TopService service, string name)
    {
        service.DisplayName = name;
        return service;
    }

    public static TopService SetDescription(this TopService service, string description)
    {
        service.Description = description;
        return service;
    }
}
