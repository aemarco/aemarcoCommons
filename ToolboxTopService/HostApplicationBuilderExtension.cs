namespace aemarcoCommons.ToolboxTopService;

//https://learn.microsoft.com/en-US/dotnet/core/extensions/workers

public static class HostApplicationBuilderExtension
{

    public static async Task<int> RunAsTopService(
        this HostApplicationBuilder app,
        Action<TopServiceBuilder> config)
    {
        //setup service
        var args = Environment.GetCommandLineArgs();
        var serviceBuilder = new TopServiceBuilder(args[0]);
        config(serviceBuilder);
        var service = serviceBuilder.Build();

        app.Services.AddWindowsService(c =>
        {
            c.ServiceName = service.ServiceName;
        });

        var command = args.Length > 1
            ? args[1].ToLower()
            : null;
        Func<Task> act = command switch
        {
            "install" => service.Install,
            "uninstall" => service.UnInstall,
            "start" => service.Start,
            "stop" => service.Stop,
            _ => RunApp
        };
        await act();

        return Environment.ExitCode;

        async Task RunApp()
        {
            await app.Build().RunAsync();
        }
    }
}


