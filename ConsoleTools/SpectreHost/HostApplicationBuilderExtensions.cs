using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace aemarcoCommons.ConsoleTools.SpectreHost;

#nullable enable

public static class HostApplicationBuilderExtensions
{

    public static async Task RunAsSpectreCommandApp(
        this HostApplicationBuilder app,
        Action<IConfigurator>? configureCommandApp = null)
    {
        var commandApp = new CommandApp(new ServiceCollectionTypeRegistrar(app.Services));
        await app.RunCommandApp(commandApp, configureCommandApp);
    }

    public static async Task RunAsSpectreCommandApp<TDefaultCommand>(
        this HostApplicationBuilder app,
        Action<IConfigurator>? configureCommandApp = null)
        where TDefaultCommand : class, ICommand
    {
        var commandApp = new CommandApp<TDefaultCommand>(new ServiceCollectionTypeRegistrar(app.Services));
        await app.RunCommandApp(commandApp, configureCommandApp);
    }



    private static async Task RunCommandApp(
        this HostApplicationBuilder app,
        ICommandApp commandApp,
        Action<IConfigurator>? configureCommandApp)
    {
        if (configureCommandApp is not null)
            commandApp.Configure(configureCommandApp);

        //so that we don´t get the startup / shutdown messages
        app.Services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

        //start our command app
        var commandAppTask = commandApp.RunAsync(Environment.GetCommandLineArgs().Skip(1).ToArray());

        IHost host = app.Build();

        //start our host wrapper
        await Task.Run(async () =>
        {
            try
            {
                var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
                await host.StartAsync();

                //wait for either our commandApp to complete, or host shuts down by Ctrl+C
                Task.WaitAll([commandAppTask], lifetime.ApplicationStopping);

                //command app has ended by itself
                Environment.ExitCode = await commandAppTask;
            }
            catch (OperationCanceledException) //when interrupted by Ctrl+C
            {
                Environment.ExitCode = 128;
            }
            catch (Exception ex) //useful if PropagateExceptions was used
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                Environment.ExitCode = 1;
            }
            finally
            {
                try
                {
                    //either way, we try a graceful shutdown
                    await host.StopAsync();
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                    Environment.ExitCode = 1;
                }
                host.Dispose();
            }
        });
    }
}