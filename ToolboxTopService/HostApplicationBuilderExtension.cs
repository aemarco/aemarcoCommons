namespace aemarcoCommons.ToolboxTopService;

//https://learn.microsoft.com/en-US/dotnet/core/extensions/workers

public static class HostApplicationBuilderExtension
{

    public static async Task<int> RunAsTopService(
        this HostApplicationBuilder app,
        Action<TopServiceBuilder> config)
    {
        //setup service
        var serviceBuilder = new TopServiceBuilder(app);
        config(serviceBuilder);
        var service = serviceBuilder.Build();

        app.Services.AddWindowsService(c => c.ServiceName = service.ServiceName);

        var args = Environment.GetCommandLineArgs();
        var command = args.Length > 1
            ? args[1].ToLower()
            : null;
        Func<Task> act = command switch
        {
            "install" => service.Install,
            "uninstall" => service.UnInstall,
            "start" => service.Start,
            "stop" => service.Stop,
            _ => () => app.Build().RunAsync()
        };
        await act();

        return Environment.ExitCode;
    }
}


