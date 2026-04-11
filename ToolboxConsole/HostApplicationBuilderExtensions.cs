using aemarcoCommons.ToolboxConsole.CommandApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Spectre.Console.Cli;
using System;
using System.Linq;

namespace aemarcoCommons.ToolboxConsole;



public static class HostApplicationBuilderExtensions
{
    private static AppTypeRegistrar? _registrar;
    extension(HostApplicationBuilder app)
    {
        public async Task RunAsSpectreCommandApp(Action<IConfigurator>? configureCommandApp = null)
        {
            _registrar = new AppTypeRegistrar(app);
            var commandApp = new Spectre.Console.Cli.CommandApp(_registrar);
            await app.RunCommandApp(
                commandApp,
                (_, x) => configureCommandApp?.Invoke(x));
        }

        public async Task RunAsSpectreCommandApp<TDefaultCommand>(Action<IConfigurator>? configureCommandApp = null)
            where TDefaultCommand : class, ICommand
        {
            _registrar = new AppTypeRegistrar(app);
            var commandApp = new CommandApp<TDefaultCommand>(_registrar);
            await app.RunCommandApp(
                commandApp,
                (_, x) => configureCommandApp?.Invoke(x));
        }

        public async Task RunAsSpectreCommandApp(Action<HostApplicationBuilder, IConfigurator>? configureCommandApp = null)
        {
            _registrar = new AppTypeRegistrar(app);
            var commandApp = new Spectre.Console.Cli.CommandApp(_registrar);
            await app.RunCommandApp(commandApp, configureCommandApp);
        }

        public async Task RunAsSpectreCommandApp<TDefaultCommand>(Action<HostApplicationBuilder, IConfigurator>? configureCommandApp = null)
            where TDefaultCommand : class, ICommand
        {
            _registrar = new AppTypeRegistrar(app);
            var commandApp = new CommandApp<TDefaultCommand>(_registrar);
            await app.RunCommandApp(commandApp, configureCommandApp);
        }

        private async Task RunCommandApp(ICommandApp commandApp,
            Action<HostApplicationBuilder, IConfigurator>? configureCommandApp)
        {
            //run configuration of command app
            commandApp.Configure(x =>
            {
                x.SetApplicationName(app.Environment.ApplicationName);
                configureCommandApp?.Invoke(app, x);
            });

            //so that we don´t get the startup / shutdown messages
            app.Services.Configure<ConsoleLifetimeOptions>(options =>
                options.SuppressStatusMessages = true);



            //start our command app
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            var commandAppTask = commandApp.RunAsync(args);
            if (commandAppTask.Status is TaskStatus.RanToCompletion)
            {
                //command app has ended by itself
                Environment.ExitCode = await commandAppTask;
                return;
            }

            if (_registrar?.Host is not { } host)
                return; //we are already done

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
                    Environment.ExitCode = -1;
                }
                finally
                {
                    await Log.CloseAndFlushAsync();
                    try
                    {
                        //either way, we try a graceful shutdown
                        await host.StopAsync();
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                        Environment.ExitCode = -1;
                    }
                    host.Dispose();
                }
            });
        }
    }
}